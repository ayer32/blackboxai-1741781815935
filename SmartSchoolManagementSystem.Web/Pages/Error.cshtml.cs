using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartSchoolManagementSystem.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorModel(ILogger<ErrorModel> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public string RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public bool IsProduction => _environment.IsProduction();
    public string ErrorMessage { get; set; }
    public string StackTrace { get; set; }

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        var exceptionHandlerPathFeature =
            HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error != null)
        {
            var exception = exceptionHandlerPathFeature.Error;
            ErrorMessage = exception.Message;
            StackTrace = exception.StackTrace;

            _logger.LogError(exception, 
                "An unhandled exception occurred while processing request {RequestId}", 
                RequestId);
        }
        else
        {
            ErrorMessage = "An error occurred while processing your request.";
            _logger.LogWarning(
                "No exception details found for error page request {RequestId}", 
                RequestId);
        }
    }
}
