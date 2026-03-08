namespace MusicRentalAutomation;

partial class BookingDialog
{
    private System.ComponentModel.IContainer? components = null;

    private TableLayoutPanel layoutPanel = null!;
    private Label clientLabel = null!;
    private Label startDateLabel = null!;
    private Label endDateLabel = null!;
    private TextBox clientNameTextBox = null!;
    private DateTimePicker startDatePicker = null!;
    private DateTimePicker endDatePicker = null!;
    private FlowLayoutPanel buttonsPanel = null!;
    private Button okButton = null!;
    private Button cancelButton = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        layoutPanel = new TableLayoutPanel();
        clientLabel = new Label();
        startDateLabel = new Label();
        endDateLabel = new Label();
        clientNameTextBox = new TextBox();
        startDatePicker = new DateTimePicker();
        endDatePicker = new DateTimePicker();
        buttonsPanel = new FlowLayoutPanel();
        okButton = new Button();
        cancelButton = new Button();

        SuspendLayout();

        Text = "Новое бронирование";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(430, 210);

        layoutPanel.Dock = DockStyle.Fill;
        layoutPanel.Padding = new Padding(10);
        layoutPanel.ColumnCount = 2;
        layoutPanel.RowCount = 4;
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        clientLabel.Text = "Клиент";
        clientLabel.TextAlign = ContentAlignment.MiddleLeft;
        clientLabel.Dock = DockStyle.Fill;

        startDateLabel.Text = "Дата начала";
        startDateLabel.TextAlign = ContentAlignment.MiddleLeft;
        startDateLabel.Dock = DockStyle.Fill;

        endDateLabel.Text = "Дата окончания";
        endDateLabel.TextAlign = ContentAlignment.MiddleLeft;
        endDateLabel.Dock = DockStyle.Fill;

        clientNameTextBox.PlaceholderText = "ФИО клиента";
        clientNameTextBox.Dock = DockStyle.Fill;

        startDatePicker.Value = DateTime.Today;
        startDatePicker.Dock = DockStyle.Fill;

        endDatePicker.Value = DateTime.Today.AddDays(3);
        endDatePicker.Dock = DockStyle.Fill;

        buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonsPanel.Dock = DockStyle.Fill;

        okButton.Text = "Создать";
        okButton.DialogResult = DialogResult.OK;
        okButton.Width = 120;

        cancelButton.Text = "Отмена";
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Width = 120;

        buttonsPanel.Controls.AddRange([okButton, cancelButton]);

        layoutPanel.Controls.Add(clientLabel, 0, 0);
        layoutPanel.Controls.Add(clientNameTextBox, 1, 0);
        layoutPanel.Controls.Add(startDateLabel, 0, 1);
        layoutPanel.Controls.Add(startDatePicker, 1, 1);
        layoutPanel.Controls.Add(endDateLabel, 0, 2);
        layoutPanel.Controls.Add(endDatePicker, 1, 2);
        layoutPanel.Controls.Add(buttonsPanel, 1, 3);

        AcceptButton = okButton;
        CancelButton = cancelButton;

        Controls.Add(layoutPanel);
        ResumeLayout(false);
    }
}
