using SmartSchoolManagementSystem.Core.Entities.Donation;

namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface IDonationRepository : IRepository<Donation>
{
    Task<IReadOnlyList<Donation>> GetDonationsByStatusAsync(DonationStatus status);
    Task<decimal> GetTotalDonationsAsync();
    Task<IReadOnlyList<Donation>> GetDonationsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<string> GenerateReceiptNumberAsync();
}
