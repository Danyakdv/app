using MusicRentalAutomation.Models;
using MusicRentalAutomation.Services;

namespace MusicRentalAutomation;

public partial class MainForm : Form
{
    private readonly RentalService _service;

    public MainForm(RentalService service)
    {
        _service = service;
        InitializeComponent();
        BindData();
    }

    private void AddInstrumentButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(instrumentNameTextBox.Text) || string.IsNullOrWhiteSpace(instrumentCategoryTextBox.Text))
        {
            MessageBox.Show("Укажите название и категорию инструмента.");
            return;
        }

        _service.AddInstrument(instrumentNameTextBox.Text.Trim(), instrumentCategoryTextBox.Text.Trim(), dailyRateNumeric.Value);
        instrumentNameTextBox.Clear();
        instrumentCategoryTextBox.Clear();
        dailyRateNumeric.Value = 1000;
    }

    private void CreateBookingButton_Click(object? sender, EventArgs e)
    {
        if (inventoryGrid.CurrentRow?.DataBoundItem is not Instrument instrument)
        {
            MessageBox.Show("Выберите инструмент на вкладке 'Учёт инвентаря'.");
            return;
        }

        using var bookingDialog = new BookingDialog();
        if (bookingDialog.ShowDialog(this) != DialogResult.OK)
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
    }

    private void CreateContractButton_Click(object? sender, EventArgs e)
    {
        if (bookingGrid.CurrentRow?.DataBoundItem is not Booking booking)
        {
            MessageBox.Show("Выберите бронирование на вкладке 'Бронирование'.");
            return;
        }

        var days = Math.Max((booking.EndDate - booking.StartDate).Days, 1);
        var instrument = _service.Instruments.FirstOrDefault(x => x.Id == booking.InstrumentId);
        var total = instrument is null ? 0 : days * instrument.DailyRate;

        _service.CreateContract(booking, total);
    }

    private void RegisterPaymentButton_Click(object? sender, EventArgs e)
    {
        if (contractGrid.CurrentRow?.DataBoundItem is not Contract contract)
        {
            MessageBox.Show("Выберите договор на вкладке 'Договоры'.");
            return;
        }

        _service.RegisterPayment(contract.Id, contract.TotalAmount, "Карта");
    }

    private void BindData()
    {
        inventoryGrid.DataSource = _service.Instruments;
        bookingGrid.DataSource = _service.Bookings;
        contractGrid.DataSource = _service.Contracts;
        paymentGrid.DataSource = _service.Payments;
    }
}
