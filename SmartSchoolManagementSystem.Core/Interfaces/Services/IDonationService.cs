using SmartSchoolManagementSystem.Core.DTOs.Donation;
using SmartSchoolManagementSystem.Core.Entities.Donation;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface IDonationService : IBaseService<Donation, DonationDto, CreateDonationDto, UpdateDonationDto>
{
    Task<IReadOnlyList<DonationDto>> GetDonationsByStatusAsync(DonationStatus status);
    Task<IReadOnlyList<DonationDto>> GetDonationsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<DonationSummaryDto> GetDonationSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<string> GenerateReceiptNumberAsync();
    Task<decimal> GetTotalDonationsAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<DonationDto> ProcessDonationAsync(CreateDonationDto createDonationDto);
    Task<DonationDto> UpdateDonationStatusAsync(Guid id, DonationStatus status);
}
