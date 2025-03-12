using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartSchoolManagementSystem.Web.Pages;

public abstract class BasePageModel : PageModel
{
    [TempData]
    public string SuccessMessage { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    [TempData]
    public string WarningMessage { get; set; }

    [TempData]
    public string InfoMessage { get; set; }

    protected void SetSuccessMessage(string message)
    {
        SuccessMessage = message;
    }

    protected void SetErrorMessage(string message)
    {
        ErrorMessage = message;
    }

    protected void SetWarningMessage(string message)
    {
        WarningMessage = message;
    }

    protected void SetInfoMessage(string message)
    {
        InfoMessage = message;
    }

    protected IActionResult RedirectToIndexWithSuccess(string message)
    {
        SetSuccessMessage(message);
        return RedirectToPage("./Index");
    }

    protected IActionResult RedirectToIndexWithError(string message)
    {
        SetErrorMessage(message);
        return RedirectToPage("./Index");
    }

    protected IActionResult RedirectToPageWithSuccess(string page, string message)
    {
        SetSuccessMessage(message);
        return RedirectToPage(page);
    }

    protected IActionResult RedirectToPageWithError(string page, string message)
    {
        SetErrorMessage(message);
        return RedirectToPage(page);
    }

    protected IActionResult HandleException(Exception ex, string userMessage = null)
    {
        // Log the exception here
        var message = userMessage ?? "An error occurred while processing your request.";
        SetErrorMessage(message);
        return RedirectToPage("./Index");
    }

    protected bool ValidateModelAndSetError()
    {
        if (ModelState.IsValid)
            return true;

        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);

        SetErrorMessage(string.Join(" ", errors));
        return false;
    }

    protected void AddModelError(string key, string message)
    {
        ModelState.AddModelError(key, message);
    }

    protected void AddModelError(string message)
    {
        ModelState.AddModelError(string.Empty, message);
    }

    protected bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }

    protected JsonResult JsonSuccess(object data = null, string message = null)
    {
        return new JsonResult(new
        {
            success = true,
            message,
            data
        });
    }

    protected JsonResult JsonError(string message = null, object data = null)
    {
        return new JsonResult(new
        {
            success = false,
            message,
            data
        });
    }

    protected string GetCurrentUserId()
    {
        // Implement user identification logic here
        return User?.Identity?.Name;
    }

    protected bool IsUserInRole(string role)
    {
        // Implement role check logic here
        return User?.IsInRole(role) ?? false;
    }

    protected virtual void OnBeforeAction()
    {
        // Hook for running code before action execution
    }

    protected virtual void OnAfterAction()
    {
        // Hook for running code after action execution
    }

    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        OnBeforeAction();
        base.OnPageHandlerExecuting(context);
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        OnAfterAction();
        base.OnPageHandlerExecuted(context);
    }
}
