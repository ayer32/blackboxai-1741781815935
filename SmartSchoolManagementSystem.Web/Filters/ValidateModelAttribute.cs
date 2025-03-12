using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartSchoolManagementSystem.Web.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            if (context.HttpContext.Request.Method == HttpMethod.Get.Method)
            {
                var tempData = ((Controller)context.Controller).TempData;
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                tempData["ErrorMessage"] = string.Join(" ", errors);

                context.Result = new RedirectResult(context.HttpContext.Request.Headers["Referer"].ToString());
            }
            else
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new JsonResult(new
                    {
                        success = false,
                        message = "Validation failed",
                        errors = errors
                    });
                }
                else
                {
                    var page = context.ActionDescriptor as Microsoft.AspNetCore.Mvc.RazorPages.CompiledPageActionDescriptor;
                    if (page != null)
                    {
                        var tempData = ((Microsoft.AspNetCore.Mvc.RazorPages.PageModel)context.Controller).TempData;
                        tempData["ErrorMessage"] = string.Join(" ", errors);
                        context.Result = new RedirectToPageResult(page.ViewEnginePath);
                    }
                    else
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            success = false,
                            message = "Validation failed",
                            errors = errors
                        });
                    }
                }
            }
        }
    }
}

public class AjaxOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
        {
            context.Result = new NotFoundResult();
        }
    }
}

public class PreventDuplicateRequestAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Method != HttpMethod.Post.Method)
        {
            await next();
            return;
        }

        var key = $"request-{context.HttpContext.Request.Path}-{context.HttpContext.Request.Method}";
        if (context.HttpContext.Session.TryGetValue(key, out _))
        {
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    message = "Duplicate request detected. Please try again."
                });
            }
            else
            {
                var tempData = ((Controller)context.Controller).TempData;
                tempData["ErrorMessage"] = "Duplicate request detected. Please try again.";
                context.Result = new RedirectResult(context.HttpContext.Request.Headers["Referer"].ToString());
            }
            return;
        }

        context.HttpContext.Session.SetString(key, DateTime.UtcNow.ToString());
        await next();
        context.HttpContext.Session.Remove(key);
    }
}

public class RequireConfirmationAttribute : ActionFilterAttribute
{
    private readonly string _message;

    public RequireConfirmationAttribute(string message = "Are you sure you want to perform this action?")
    {
        _message = message;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var confirmed = context.HttpContext.Request.Headers["X-Confirmation"].ToString();
        if (string.IsNullOrEmpty(confirmed) || confirmed.ToLower() != "true")
        {
            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    requireConfirmation = true,
                    message = _message
                });
            }
            else
            {
                var tempData = ((Controller)context.Controller).TempData;
                tempData["WarningMessage"] = _message;
                context.Result = new RedirectResult(context.HttpContext.Request.Headers["Referer"].ToString());
            }
        }
    }
}
