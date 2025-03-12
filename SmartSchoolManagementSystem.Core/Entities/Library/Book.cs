namespace SmartSchoolManagementSystem.Core.Entities.Library;

public class Book : BaseEntity
{
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string QRCode { get; set; } = string.Empty;
    public ICollection<BookLending> BookLendings { get; set; } = new List<BookLending>();
}
