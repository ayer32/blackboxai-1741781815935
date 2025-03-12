using SmartSchoolManagementSystem.Core.DTOs.Library;
using SmartSchoolManagementSystem.Core.Entities.Library;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface IBookService : IBaseService<Book, BookDto, CreateBookDto, UpdateBookDto>
{
    Task<BookDto> GetBookByISBNAsync(string isbn);
    Task<IReadOnlyList<BookDto>> SearchBooksAsync(string searchTerm);
    Task<IReadOnlyList<BookDto>> GetBooksByAuthorAsync(string author);
    Task<IReadOnlyList<BookDto>> GetBooksByCategoryAsync(string category);
    Task<IReadOnlyList<BookDto>> GetAvailableBooksAsync();
    Task<bool> UpdateBookAvailabilityAsync(Guid bookId, int adjustment);
    Task<string> GenerateQRCodeAsync(string isbn);
    Task<BookSummaryDto> GetBookSummaryAsync();
    Task<IReadOnlyList<PopularBookDto>> GetPopularBooksAsync(int count = 10);
    Task<bool> IsISBNUniqueAsync(string isbn, Guid? excludeId = null);
}
