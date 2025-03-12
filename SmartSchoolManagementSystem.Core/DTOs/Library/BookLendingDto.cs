using SmartSchoolManagementSystem.Core.Entities.Library;

namespace SmartSchoolManagementSystem.Core.DTOs.Library;

public class BookLendingDto : BaseDto
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string BookISBN { get; set; } = string.Empty;
    public Guid BorrowerId { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public decimal? FineAmount { get; set; }
    public bool IsFineCollected { get; set; }
    public LendingStatus Status { get; set; }
}

public class CreateBookLendingDto
{
    public Guid BookId { get; set; }
    public Guid BorrowerId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
}

public class UpdateBookLendingDto
{
    public DateTime? ReturnDate { get; set; }
    public decimal? FineAmount { get; set; }
    public bool IsFineCollected { get; set; }
    public LendingStatus Status { get; set; }
}

public class LendingSummaryDto
{
    public int TotalActiveLendings { get; set; }
    public int OverdueLendings { get; set; }
    public decimal TotalFinesCollected { get; set; }
    public decimal PendingFines { get; set; }
    public Dictionary<LendingStatus, int> LendingsByStatus { get; set; } = new();
    public List<MonthlyLendingDto> MonthlyLendings { get; set; } = new();
}

public class MonthlyLendingDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int LendingCount { get; set; }
    public int ReturnCount { get; set; }
    public decimal FinesCollected { get; set; }
}
