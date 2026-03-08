namespace MusicRentalAutomation;

internal partial class BookingDialog : Form
{
    public string ClientName => clientNameTextBox.Text;
    public DateTime StartDate => startDatePicker.Value.Date;
    public DateTime EndDate => endDatePicker.Value.Date;

    public BookingDialog()
    {
        InitializeComponent();
    }
}
