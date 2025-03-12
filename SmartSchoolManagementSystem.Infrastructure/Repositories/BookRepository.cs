using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities.Library;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Data;

namespace SmartSchoolManagementSystem.Infrastructure.Repositories;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Book?> GetBookByISBNAsync(string isbn)
    {
        return await _entities
            .FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<IReadOnlyList<Book>> SearchBooksAsync(string searchTerm)
    {
        return await _entities
            .Where(b => 
                b.Title.Contains(searchTerm) ||
                b.Author.Contains(searchTerm) ||
                b.ISBN.Contains(searchTerm) ||
                b.Publisher.Contains(searchTerm) ||
                b.Category.Contains(searchTerm))
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Book>> GetBooksByAuthorAsync(string author)
    {
        return await _entities
            .Where(b => b.Author.Contains(author))
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Book>> GetBooksByCategoryAsync(string category)
    {
        return await _entities
            .Where(b => b.Category == category)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Book>> GetAvailableBooksAsync()
    {
        return await _entities
            .Where(b => b.AvailableCopies > 0)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<bool> UpdateBookAvailabilityAsync(Guid bookId, int adjustment)
    {
        var book = await _entities.FindAsync(bookId);
        if (book == null) return false;

        var newAvailableCopies = book.AvailableCopies + adjustment;
        if (newAvailableCopies < 0 || newAvailableCopies > book.TotalCopies)
            return false;

        book.AvailableCopies = newAvailableCopies;
        return true;
    }

    public async Task<string> GenerateQRCodeAsync(string isbn)
    {
        // In a real implementation, this would generate a QR code
        // For now, we'll just return a placeholder string
        return $"QR-{isbn}-{DateTime.UtcNow.Ticks}";
    }

    public override async Task<IReadOnlyList<Book>> GetAllAsync()
    {
        return await _entities
            .Include(b => b.BookLendings)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }
}
