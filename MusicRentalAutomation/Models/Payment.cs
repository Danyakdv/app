namespace MusicRentalAutomation.Models;

public class Payment
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public string Method { get; set; } = "Карта";
    public string PaymentStatus { get; set; } = "Оплачен";
}
