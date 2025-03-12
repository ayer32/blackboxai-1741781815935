using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities.Donation;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Data;

namespace SmartSchoolManagementSystem.Infrastructure.Repositories;

public class DonationRepository : BaseRepository<Donation>, IDonationRepository
{
    public DonationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Donation>> GetDonationsByStatusAsync(DonationStatus status)
    {
        return await _entities
            .Where(d => d.Status == status)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalDonationsAsync()
    {
        return await _entities
            .Where(d => d.Status == DonationStatus.Completed)
            .SumAsync(d => d.Amount);
    }

    public async Task<IReadOnlyList<Donation>> GetDonationsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _entities
            .Where(d => d.CreatedAt >= startDate && d.CreatedAt <= endDate)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<string> GenerateReceiptNumberAsync()
    {
        var lastDonation = await _entities
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync();

        string prefix = "DON";
        int number = 1;

        if (lastDonation?.ReceiptNumber != null)
        {
            string lastNumber = lastDonation.ReceiptNumber.Substring(3);
            if (int.TryParse(lastNumber, out int lastSeq))
            {
                number = lastSeq + 1;
            }
        }

        return $"{prefix}{number:D6}";
    }

    public override async Task<IReadOnlyList<Donation>> GetAllAsync()
    {
        return await _entities
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }
}
