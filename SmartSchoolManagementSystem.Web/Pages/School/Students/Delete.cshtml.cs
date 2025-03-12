using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Web.Pages.School.Students;

public class DeleteModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly IBookLendingService _bookLendingService;

    public DeleteModel(
        IStudentService studentService,
        IBookLendingService bookLendingService)
    {
        _studentService = studentService;
        _bookLendingService = bookLendingService;
    }

    [BindProperty]
    public StudentDto Student { get; set; }

    public bool HasActiveBookLendings { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Student = await _studentService.GetByIdAsync(id);
        if (Student == null)
        {
            return NotFound();
        }

        // Check if student has any active book lendings
        var lendings = await _bookLendingService.GetLendingsByBorrowerAsync(id);
        HasActiveBookLendings = lendings.Any(l => l.Status == "Borrowed");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var student = await _studentService.GetByIdAsync(Student.Id);
        if (student == null)
        {
            return NotFound();
        }

        // Double-check for active book lendings before deletion
        var lendings = await _bookLendingService.GetLendingsByBorrowerAsync(Student.Id);
        if (lendings.Any(l => l.Status == "Borrowed"))
        {
            ModelState.AddModelError(string.Empty, "Cannot delete student with active book lendings.");
            HasActiveBookLendings = true;
            return Page();
        }

        try
        {
            await _studentService.DeleteAsync(Student.Id);
            TempData["SuccessMessage"] = "Student deleted successfully.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the student: " + ex.Message);
            return Page();
        }
    }
}
