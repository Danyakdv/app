namespace MusicRentalAutomation.Models;

public class Contract
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public DateTime SignedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string ContractStatus { get; set; } = "Действует";
}
