namespace SmartSchoolManagementSystem.Core.Entities.Library;

public class BookLending : BaseEntity
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public Guid BorrowerId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public decimal? FineAmount { get; set; }
    public bool IsFineCollected { get; set; }
    public LendingStatus Status { get; set; }
}

public enum LendingStatus
{
    Borrowed,
    Returned,
    Overdue,
    Lost
}
