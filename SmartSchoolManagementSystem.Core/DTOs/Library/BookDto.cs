namespace SmartSchoolManagementSystem.Core.DTOs.Library;

public class BookDto : BaseDto
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
}

public class CreateBookDto
{
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
}

public class UpdateBookDto
{
    public string Location { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
}

public class BookSummaryDto
{
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int BorrowedBooks { get; set; }
    public int OverdueBooks { get; set; }
    public Dictionary<string, int> BooksByCategory { get; set; } = new();
    public List<PopularBookDto> PopularBooks { get; set; } = new();
}

public class PopularBookDto
{
    public Guid BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int BorrowCount { get; set; }
}
