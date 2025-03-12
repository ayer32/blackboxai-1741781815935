using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Web.Pages.School.Students;

public class EditModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly IClassService _classService;

    public EditModel(IStudentService studentService, IClassService classService)
    {
        _studentService = studentService;
        _classService = classService;
    }

    [BindProperty]
    public UpdateStudentDto Student { get; set; }

    public SelectList ClassList { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        // Map StudentDto to UpdateStudentDto
        Student = new UpdateStudentDto
        {
            Id = student.Id,
            StudentId = student.StudentId,
            FirstName = student.FirstName,
            LastName = student.LastName,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            Email = student.Email,
            PhoneNumber = student.PhoneNumber,
            Address = student.Address,
            ClassId = student.ClassId
        };

        var classes = await _classService.GetAllAsync();
        ClassList = new SelectList(classes, "Id", "Name");

        return Page();
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
            // Check if email is unique (excluding current student)
            if (!await _studentService.IsEmailUniqueAsync(Student.Email, Student.Id))
            {
                ModelState.AddModelError("Student.Email", "This email is already in use.");
                var classes = await _classService.GetAllAsync();
                ClassList = new SelectList(classes, "Id", "Name");
                return Page();
            }

            await _studentService.UpdateAsync(Student.Id, Student);
            TempData["SuccessMessage"] = "Student updated successfully.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while updating the student: " + ex.Message);
            var classes = await _classService.GetAllAsync();
            ClassList = new SelectList(classes, "Id", "Name");
            return Page();
        }
    }
}
