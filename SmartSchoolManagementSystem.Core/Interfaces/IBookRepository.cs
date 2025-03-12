using SmartSchoolManagementSystem.Core.Entities.Library;

namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetBookByISBNAsync(string isbn);
    Task<IReadOnlyList<Book>> SearchBooksAsync(string searchTerm);
    Task<IReadOnlyList<Book>> GetBooksByAuthorAsync(string author);
    Task<IReadOnlyList<Book>> GetBooksByCategoryAsync(string category);
    Task<IReadOnlyList<Book>> GetAvailableBooksAsync();
    Task<bool> UpdateBookAvailabilityAsync(Guid bookId, int adjustment);
    Task<string> GenerateQRCodeAsync(string isbn);
}
