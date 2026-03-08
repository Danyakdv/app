namespace MusicRentalAutomation.Models;

public class Booking
{
    public int Id { get; set; }
    public int InstrumentId { get; set; }
    public string ClientFullName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string BookingStatus { get; set; } = "Забронировано";
}
