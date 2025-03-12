using SmartSchoolManagementSystem.Core.Entities.Donation;

namespace SmartSchoolManagementSystem.Core.DTOs.Donation;

public class DonationDto : BaseDto
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

public class CreateDonationDto
{
    public decimal Amount { get; set; }
    public string DonorName { get; set; } = string.Empty;
    public string? DonorEmail { get; set; }
    public string? DonorPhone { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateDonationDto
{
    public DonationStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? ReceiptNumber { get; set; }
}

public class DonationSummaryDto
{
    public decimal TotalAmount { get; set; }
    public int TotalDonations { get; set; }
    public Dictionary<DonationStatus, int> DonationsByStatus { get; set; } = new();
    public List<MonthlyDonationDto> MonthlyDonations { get; set; } = new();
}

public class MonthlyDonationDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalAmount { get; set; }
    public int DonationCount { get; set; }
}
