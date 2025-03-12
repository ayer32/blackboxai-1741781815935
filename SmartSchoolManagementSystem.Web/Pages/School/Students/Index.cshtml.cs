using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Web.Pages.School.Students;

public class IndexModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly IClassService _classService;
    private const int PageSize = 10;

    public IndexModel(IStudentService studentService, IClassService classService)
    {
        _studentService = studentService;
        _classService = classService;
    }

    public IReadOnlyList<StudentDto> Students { get; set; } = new List<StudentDto>();
    public SelectList ClassList { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? ClassId { get; set; }

    public async Task OnGetAsync(int pageNumber = 1)
    {
        CurrentPage = pageNumber;

        // Get all classes for the dropdown
        var classes = await _classService.GetAllAsync();
        ClassList = new SelectList(classes, "Id", "Name");

        // Get students with filtering and pagination
        var allStudents = await _studentService.GetAllAsync();

        // Apply filters
        var filteredStudents = allStudents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            filteredStudents = filteredStudents.Where(s =>
                s.FirstName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.LastName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.StudentId.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (ClassId.HasValue)
        {
            filteredStudents = filteredStudents.Where(s => s.ClassId == ClassId.Value);
        }

        // Calculate pagination
        var totalStudents = filteredStudents.Count();
        TotalPages = (int)Math.Ceiling(totalStudents / (double)PageSize);
        CurrentPage = Math.Max(1, Math.Min(pageNumber, TotalPages));

        // Apply pagination
        Students = filteredStudents
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }
}
