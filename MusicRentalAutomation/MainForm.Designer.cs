namespace MusicRentalAutomation;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;

    private TabControl mainTabControl = null!;
    private TabPage inventoryTabPage = null!;
    private TabPage bookingsTabPage = null!;
    private TabPage contractsTabPage = null!;
    private TabPage paymentsTabPage = null!;

    private DataGridView inventoryGrid = null!;
    private DataGridView bookingGrid = null!;
    private DataGridView contractGrid = null!;
    private DataGridView paymentGrid = null!;

    private FlowLayoutPanel inventoryControlsPanel = null!;
    private TextBox instrumentNameTextBox = null!;
    private TextBox instrumentCategoryTextBox = null!;
    private NumericUpDown dailyRateNumeric = null!;
    private Button addInstrumentButton = null!;

    private Button createBookingButton = null!;
    private Button createContractButton = null!;
    private Button registerPaymentButton = null!;

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

        mainTabControl = new TabControl();
        inventoryTabPage = new TabPage();
        bookingsTabPage = new TabPage();
        contractsTabPage = new TabPage();
        paymentsTabPage = new TabPage();

        inventoryGrid = new DataGridView();
        bookingGrid = new DataGridView();
        contractGrid = new DataGridView();
        paymentGrid = new DataGridView();

        inventoryControlsPanel = new FlowLayoutPanel();
        instrumentNameTextBox = new TextBox();
        instrumentCategoryTextBox = new TextBox();
        dailyRateNumeric = new NumericUpDown();
        addInstrumentButton = new Button();

        createBookingButton = new Button();
        createContractButton = new Button();
        registerPaymentButton = new Button();

        SuspendLayout();

        Text = "Автоматизация аренды музыкальных инструментов (PostgreSQL)";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1200, 700);

        mainTabControl.Dock = DockStyle.Fill;
        mainTabControl.TabPages.AddRange([inventoryTabPage, bookingsTabPage, contractsTabPage, paymentsTabPage]);

        inventoryTabPage.Text = "Учёт инвентаря";
        bookingsTabPage.Text = "Бронирование";
        contractsTabPage.Text = "Договоры";
        paymentsTabPage.Text = "Платежи";

        ConfigureGrid(inventoryGrid);
        ConfigureGrid(bookingGrid);
        ConfigureGrid(contractGrid);
        ConfigureGrid(paymentGrid);

        inventoryControlsPanel.Dock = DockStyle.Top;
        inventoryControlsPanel.Padding = new Padding(10);
        inventoryControlsPanel.Height = 56;

        instrumentNameTextBox.PlaceholderText = "Название";
        instrumentNameTextBox.Width = 220;

        instrumentCategoryTextBox.PlaceholderText = "Категория";
        instrumentCategoryTextBox.Width = 180;

        dailyRateNumeric.DecimalPlaces = 2;
        dailyRateNumeric.Minimum = 100;
        dailyRateNumeric.Maximum = 50000;
        dailyRateNumeric.Value = 1000;
        dailyRateNumeric.Width = 120;

        addInstrumentButton.Text = "Добавить инструмент";
        addInstrumentButton.AutoSize = true;
        addInstrumentButton.Click += AddInstrumentButton_Click;

        inventoryControlsPanel.Controls.AddRange([instrumentNameTextBox, instrumentCategoryTextBox, dailyRateNumeric, addInstrumentButton]);

        inventoryTabPage.Controls.Add(inventoryGrid);
        inventoryTabPage.Controls.Add(inventoryControlsPanel);

        createBookingButton.Dock = DockStyle.Top;
        createBookingButton.Height = 36;
        createBookingButton.Text = "Создать бронирование из выбранного инструмента";
        createBookingButton.Click += CreateBookingButton_Click;
        bookingsTabPage.Controls.Add(bookingGrid);
        bookingsTabPage.Controls.Add(createBookingButton);

        createContractButton.Dock = DockStyle.Top;
        createContractButton.Height = 36;
        createContractButton.Text = "Сформировать договор по выбранному бронированию";
        createContractButton.Click += CreateContractButton_Click;
        contractsTabPage.Controls.Add(contractGrid);
        contractsTabPage.Controls.Add(createContractButton);

        registerPaymentButton.Dock = DockStyle.Top;
        registerPaymentButton.Height = 36;
        registerPaymentButton.Text = "Зарегистрировать оплату по выбранному договору";
        registerPaymentButton.Click += RegisterPaymentButton_Click;
        paymentsTabPage.Controls.Add(paymentGrid);
        paymentsTabPage.Controls.Add(registerPaymentButton);

        Controls.Add(mainTabControl);
        ResumeLayout(false);
    }

    private static void ConfigureGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AutoGenerateColumns = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
    }
}
