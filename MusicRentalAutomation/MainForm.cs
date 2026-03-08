using MusicRentalAutomation.Models;
using MusicRentalAutomation.Services;

namespace MusicRentalAutomation;

public class MainForm : Form
{
    private readonly RentalService _service;

    private readonly DataGridView _inventoryGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView _bookingGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView _contractGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };
    private readonly DataGridView _paymentGrid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true };

    private readonly TextBox _instrumentName = new() { PlaceholderText = "Название" };
    private readonly TextBox _instrumentCategory = new() { PlaceholderText = "Категория" };
    private readonly NumericUpDown _dailyRate = new() { DecimalPlaces = 2, Maximum = 50000, Minimum = 100, Value = 1000 };

    public MainForm(RentalService service)
    {
        _service = service;

        Text = "Автоматизация аренды музыкальных инструментов (PostgreSQL)";
        Width = 1200;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(CreateInventoryTab());
        tabs.TabPages.Add(CreateBookingsTab());
        tabs.TabPages.Add(CreateContractsTab());
        tabs.TabPages.Add(CreatePaymentsTab());

        Controls.Add(tabs);
        BindData();
    }

    private TabPage CreateInventoryTab()
    {
        var tab = new TabPage("Учёт инвентаря");

        var addButton = new Button { Text = "Добавить инструмент", AutoSize = true };
        addButton.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_instrumentName.Text) || string.IsNullOrWhiteSpace(_instrumentCategory.Text))
            {
                MessageBox.Show("Укажите название и категорию инструмента.");
                return;
            }

            _service.AddInstrument(_instrumentName.Text.Trim(), _instrumentCategory.Text.Trim(), _dailyRate.Value);
            _instrumentName.Clear();
            _instrumentCategory.Clear();
            _dailyRate.Value = 1000;
        };

        var controlsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 50,
            Padding = new Padding(10),
            AutoSize = true
        };

        controlsPanel.Controls.AddRange([_instrumentName, _instrumentCategory, _dailyRate, addButton]);

        tab.Controls.Add(_inventoryGrid);
        tab.Controls.Add(controlsPanel);
        return tab;
    }

    private TabPage CreateBookingsTab()
    {
        var tab = new TabPage("Бронирование");
        var createBookingButton = new Button { Text = "Создать бронирование из выбранного инструмента", Dock = DockStyle.Top, Height = 36 };

        createBookingButton.Click += (_, _) =>
        {
            if (_inventoryGrid.CurrentRow?.DataBoundItem is not Instrument instrument)
            {
                MessageBox.Show("Выберите инструмент на вкладке 'Учёт инвентаря'.");
                return;
            }

            using var bookingDialog = new BookingDialog();
            if (bookingDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(bookingDialog.ClientName))
            {
                MessageBox.Show("Введите ФИО клиента.");
                return;
            }

            if (bookingDialog.EndDate < bookingDialog.StartDate)
            {
                MessageBox.Show("Дата окончания не может быть раньше даты начала.");
                return;
            }

            _service.AddBooking(
                instrument.Id,
                bookingDialog.ClientName.Trim(),
                bookingDialog.StartDate,
                bookingDialog.EndDate);
        };

        tab.Controls.Add(_bookingGrid);
        tab.Controls.Add(createBookingButton);
        return tab;
    }

    private TabPage CreateContractsTab()
    {
        var tab = new TabPage("Договоры");

        var createContractButton = new Button { Text = "Сформировать договор по выбранному бронированию", Dock = DockStyle.Top, Height = 36 };
        createContractButton.Click += (_, _) =>
        {
            if (_bookingGrid.CurrentRow?.DataBoundItem is not Booking booking)
            {
                MessageBox.Show("Выберите бронирование на вкладке 'Бронирование'.");
                return;
            }

            var days = Math.Max((booking.EndDate - booking.StartDate).Days, 1);
            var instrument = _service.Instruments.FirstOrDefault(x => x.Id == booking.InstrumentId);
            var total = instrument is null ? 0 : days * instrument.DailyRate;

            _service.CreateContract(booking, total);
        };

        tab.Controls.Add(_contractGrid);
        tab.Controls.Add(createContractButton);
        return tab;
    }

    private TabPage CreatePaymentsTab()
    {
        var tab = new TabPage("Платежи");

        var registerPaymentButton = new Button { Text = "Зарегистрировать оплату по выбранному договору", Dock = DockStyle.Top, Height = 36 };
        registerPaymentButton.Click += (_, _) =>
        {
            if (_contractGrid.CurrentRow?.DataBoundItem is not Contract contract)
            {
                MessageBox.Show("Выберите договор на вкладке 'Договоры'.");
                return;
            }

            _service.RegisterPayment(contract.Id, contract.TotalAmount, "Карта");
        };

        tab.Controls.Add(_paymentGrid);
        tab.Controls.Add(registerPaymentButton);
        return tab;
    }

    private void BindData()
    {
        _inventoryGrid.DataSource = _service.Instruments;
        _bookingGrid.DataSource = _service.Bookings;
        _contractGrid.DataSource = _service.Contracts;
        _paymentGrid.DataSource = _service.Payments;
    }
}

internal sealed class BookingDialog : Form
{
    private readonly TextBox _nameText = new() { PlaceholderText = "ФИО клиента", Width = 280 };
    private readonly DateTimePicker _from = new() { Value = DateTime.Today };
    private readonly DateTimePicker _to = new() { Value = DateTime.Today.AddDays(3) };

    public string ClientName => _nameText.Text;
    public DateTime StartDate => _from.Value.Date;
    public DateTime EndDate => _to.Value.Date;

    public BookingDialog()
    {
        Text = "Новое бронирование";
        Width = 430;
        Height = 210;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;

        var okButton = new Button { Text = "Создать", DialogResult = DialogResult.OK, Width = 120 };
        var cancelButton = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 120 };

        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 2,
            Padding = new Padding(10),
            AutoSize = true
        };

        panel.Controls.Add(new Label { Text = "Клиент", AutoSize = true }, 0, 0);
        panel.Controls.Add(_nameText, 1, 0);
        panel.Controls.Add(new Label { Text = "Дата начала", AutoSize = true }, 0, 1);
        panel.Controls.Add(_from, 1, 1);
        panel.Controls.Add(new Label { Text = "Дата окончания", AutoSize = true }, 0, 2);
        panel.Controls.Add(_to, 1, 2);

        var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
        buttons.Controls.AddRange([okButton, cancelButton]);
        panel.Controls.Add(buttons, 1, 3);

        AcceptButton = okButton;
        CancelButton = cancelButton;
        Controls.Add(panel);
    }
}
