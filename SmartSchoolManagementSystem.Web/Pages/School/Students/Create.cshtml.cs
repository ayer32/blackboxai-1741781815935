using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Web.Pages.School.Students;

public class CreateModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly IClassService _classService;

    public CreateModel(IStudentService studentService, IClassService classService)
    {
        _studentService = studentService;
        _classService = classService;
    }

    [BindProperty]
    public CreateStudentDto Student { get; set; } = new();

    public SelectList ClassList { get; set; }

    public async Task OnGetAsync()
    {
        var classes = await _classService.GetAllAsync();
        ClassList = new SelectList(classes, "Id", "Name");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var classes = await _classService.GetAllAsync();
            ClassList = new SelectList(classes, "Id", "Name");
            return Page();
        }

        try
        {
            // Check if email is unique
            if (!await _studentService.IsEmailUniqueAsync(Student.Email))
            {
                ModelState.AddModelError("Student.Email", "This email is already in use.");
                var classes = await _classService.GetAllAsync();
                ClassList = new SelectList(classes, "Id", "Name");
                return Page();
            }

            await _studentService.CreateAsync(Student);
            TempData["SuccessMessage"] = "Student created successfully.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while creating the student: " + ex.Message);
            var classes = await _classService.GetAllAsync();
            ClassList = new SelectList(classes, "Id", "Name");
            return Page();
        }
    }
}
