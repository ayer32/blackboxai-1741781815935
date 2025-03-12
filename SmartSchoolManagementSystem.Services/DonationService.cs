using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs.Donation;
using SmartSchoolManagementSystem.Core.Entities.Donation;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public class DonationService : BaseService<Donation, DonationDto, CreateDonationDto, UpdateDonationDto>, IDonationService
{
    private readonly IDonationRepository _donationRepository;

    public DonationService(
        IDonationRepository donationRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : base(donationRepository, unitOfWork, mapper)
    {
        _donationRepository = donationRepository;
    }

    public async Task<IReadOnlyList<DonationDto>> GetDonationsByStatusAsync(DonationStatus status)
    {
        var donations = await _donationRepository.GetDonationsByStatusAsync(status);
        return _mapper.Map<IReadOnlyList<DonationDto>>(donations);
    }

    public async Task<IReadOnlyList<DonationDto>> GetDonationsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var donations = await _donationRepository.GetDonationsByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IReadOnlyList<DonationDto>>(donations);
    }

    public async Task<DonationSummaryDto> GetDonationSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var donations = startDate.HasValue && endDate.HasValue
            ? await _donationRepository.GetDonationsByDateRangeAsync(startDate.Value, endDate.Value)
            : await _donationRepository.GetAllAsync();

        var summary = new DonationSummaryDto
        {
            TotalAmount = donations.Where(d => d.Status == DonationStatus.Completed).Sum(d => d.Amount),
            TotalDonations = donations.Count,
            DonationsByStatus = donations
                .GroupBy(d => d.Status)
                .ToDictionary(g => g.Key, g => g.Count()),
            MonthlyDonations = donations
                .GroupBy(d => new { d.CreatedAt.Year, d.CreatedAt.Month })
                .Select(g => new MonthlyDonationDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalAmount = g.Where(d => d.Status == DonationStatus.Completed).Sum(d => d.Amount),
                    DonationCount = g.Count()
                })
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .ToList()
        };

        return summary;
    }

    public async Task<string> GenerateReceiptNumberAsync()
    {
        return await _donationRepository.GenerateReceiptNumberAsync();
    }

    public async Task<decimal> GetTotalDonationsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        if (!startDate.HasValue || !endDate.HasValue)
            return await _donationRepository.GetTotalDonationsAsync();

        var donations = await _donationRepository.GetDonationsByDateRangeAsync(startDate.Value, endDate.Value);
        return donations.Where(d => d.Status == DonationStatus.Completed).Sum(d => d.Amount);
    }

    public async Task<DonationDto> ProcessDonationAsync(CreateDonationDto createDonationDto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var donation = _mapper.Map<Donation>(createDonationDto);
            donation.Status = DonationStatus.Pending;
            donation.ReceiptNumber = await GenerateReceiptNumberAsync();

            await _donationRepository.AddAsync(donation);
            await _unitOfWork.CompleteAsync();

            // Additional processing logic can be added here
            // For example, sending confirmation emails, generating receipts, etc.

            donation.Status = DonationStatus.Completed;
            await _donationRepository.UpdateAsync(donation);
            await _unitOfWork.CompleteAsync();

            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<DonationDto>(donation);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<DonationDto> UpdateDonationStatusAsync(Guid id, DonationStatus status)
    {
        var donation = await _donationRepository.GetByIdAsync(id);
        if (donation == null)
            throw new KeyNotFoundException($"Donation with ID {id} was not found.");

        donation.Status = status;
        await _donationRepository.UpdateAsync(donation);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<DonationDto>(donation);
    }
}
