namespace SmartSchoolManagementSystem.Core.Entities.Donation;

public class Donation : BaseEntity
{
    public decimal Amount { get; set; }
    public string DonorName { get; set; } = string.Empty;
    public string? DonorEmail { get; set; }
    public string? DonorPhone { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DonationStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptNumber { get; set; }
}

public enum DonationStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}
