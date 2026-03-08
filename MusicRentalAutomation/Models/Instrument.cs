namespace MusicRentalAutomation.Models;

public class Instrument
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public string Status { get; set; } = "Доступен";
}
