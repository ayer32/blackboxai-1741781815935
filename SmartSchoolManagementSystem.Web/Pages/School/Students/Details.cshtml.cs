using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartSchoolManagementSystem.Core.DTOs.Library;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Web.Pages.School.Students;

public class DetailsModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly IBookLendingService _bookLendingService;

    public DetailsModel(
        IStudentService studentService,
        IBookLendingService bookLendingService)
    {
        _studentService = studentService;
        _bookLendingService = bookLendingService;
    }

    public StudentDto Student { get; set; }
    public IReadOnlyList<BookLendingDto> BookLendings { get; set; }
    public AttendanceStatsDto AttendanceStats { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Student = await _studentService.GetByIdAsync(id);
        if (Student == null)
        {
            return NotFound();
        }

        // Get book lending history
        BookLendings = await _studentService.GetStudentLendingHistoryAsync(id);

        // Calculate attendance statistics for the current month
        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var attendance = await _studentService.GetStudentAttendanceHistoryAsync(id, startDate, endDate);

        AttendanceStats = new AttendanceStatsDto
        {
            TotalDays = attendance.Count,
            PresentDays = attendance.Count(a => a.IsPresent),
            AbsentDays = attendance.Count(a => !a.IsPresent),
            AttendancePercentage = attendance.Any() 
                ? (double)attendance.Count(a => a.IsPresent) / attendance.Count * 100 
                : 0
        };

        return Page();
    }
}

public class AttendanceStatsDto
{
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public double AttendancePercentage { get; set; }
}
