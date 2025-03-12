using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs.Library;
using SmartSchoolManagementSystem.Core.Entities.Library;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public class BookService : BaseService<Book, BookDto, CreateBookDto, UpdateBookDto>, IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookLendingRepository _bookLendingRepository;

    public BookService(
        IBookRepository bookRepository,
        IBookLendingRepository bookLendingRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : base(bookRepository, unitOfWork, mapper)
    {
        _bookRepository = bookRepository;
        _bookLendingRepository = bookLendingRepository;
    }

    public async Task<BookDto> GetBookByISBNAsync(string isbn)
    {
        var book = await _bookRepository.GetBookByISBNAsync(isbn);
        if (book == null)
            throw new KeyNotFoundException($"Book with ISBN {isbn} was not found.");

        return _mapper.Map<BookDto>(book);
    }

    public async Task<IReadOnlyList<BookDto>> SearchBooksAsync(string searchTerm)
    {
        var books = await _bookRepository.SearchBooksAsync(searchTerm);
        return _mapper.Map<IReadOnlyList<BookDto>>(books);
    }

    public async Task<IReadOnlyList<BookDto>> GetBooksByAuthorAsync(string author)
    {
        var books = await _bookRepository.GetBooksByAuthorAsync(author);
        return _mapper.Map<IReadOnlyList<BookDto>>(books);
    }

    public async Task<IReadOnlyList<BookDto>> GetBooksByCategoryAsync(string category)
    {
        var books = await _bookRepository.GetBooksByCategoryAsync(category);
        return _mapper.Map<IReadOnlyList<BookDto>>(books);
    }

    public async Task<IReadOnlyList<BookDto>> GetAvailableBooksAsync()
    {
        var books = await _bookRepository.GetAvailableBooksAsync();
        return _mapper.Map<IReadOnlyList<BookDto>>(books);
    }

    public async Task<bool> UpdateBookAvailabilityAsync(Guid bookId, int adjustment)
    {
        return await _bookRepository.UpdateBookAvailabilityAsync(bookId, adjustment);
    }

    public async Task<string> GenerateQRCodeAsync(string isbn)
    {
        return await _bookRepository.GenerateQRCodeAsync(isbn);
    }

    public async Task<BookSummaryDto> GetBookSummaryAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        var activeLendings = await _bookLendingRepository.GetActiveLendingsAsync();
        var overdueLendings = await _bookLendingRepository.GetOverdueLendingsAsync();

        var summary = new BookSummaryDto
        {
            TotalBooks = books.Sum(b => b.TotalCopies),
            AvailableBooks = books.Sum(b => b.AvailableCopies),
            BorrowedBooks = activeLendings.Count,
            OverdueBooks = overdueLendings.Count,
            BooksByCategory = books
                .GroupBy(b => b.Category)
                .ToDictionary(g => g.Key, g => g.Count()),
            PopularBooks = await GetPopularBooksAsync()
        };

        return summary;
    }

    public async Task<IReadOnlyList<PopularBookDto>> GetPopularBooksAsync(int count = 10)
    {
        var books = await _bookRepository.GetAllAsync();
        var popularBooks = books
            .OrderByDescending(b => b.BookLendings.Count)
            .Take(count)
            .Select(b => new PopularBookDto
            {
                BookId = b.Id,
                Title = b.Title,
                Author = b.Author,
                BorrowCount = b.BookLendings.Count
            })
            .ToList();

        return popularBooks;
    }

    public async Task<bool> IsISBNUniqueAsync(string isbn, Guid? excludeId = null)
    {
        var existingBook = await _bookRepository.GetBookByISBNAsync(isbn);
        return existingBook == null || (excludeId.HasValue && existingBook.Id == excludeId.Value);
    }

    public override async Task<BookDto> CreateAsync(CreateBookDto createDto)
    {
        if (!await IsISBNUniqueAsync(createDto.ISBN))
            throw new InvalidOperationException($"A book with ISBN {createDto.ISBN} already exists.");

        var book = _mapper.Map<Book>(createDto);
        book.AvailableCopies = createDto.TotalCopies;
        book.QRCode = await GenerateQRCodeAsync(createDto.ISBN);

        await _bookRepository.AddAsync(book);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<BookDto>(book);
    }
}
