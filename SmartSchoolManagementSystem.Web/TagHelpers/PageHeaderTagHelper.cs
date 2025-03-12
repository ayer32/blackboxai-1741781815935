using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SmartSchoolManagementSystem.Web.TagHelpers;

[HtmlTargetElement("page-header")]
public class PageHeaderTagHelper : TagHelper
{
    [HtmlAttributeName("title")]
    public string Title { get; set; }

    [HtmlAttributeName("subtitle")]
    public string Subtitle { get; set; }

    [HtmlAttributeName("icon")]
    public string Icon { get; set; }

    [HtmlAttributeName("back-url")]
    public string BackUrl { get; set; }

    [HtmlAttributeName("create-url")]
    public string CreateUrl { get; set; }

    [HtmlAttributeName("create-text")]
    public string CreateText { get; set; } = "Create New";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "page-header mb-4");

        var content = $@"
            <div class=""d-flex justify-content-between align-items-center"">
                <div>
                    <h2 class=""mb-1"">
                        {(string.IsNullOrEmpty(Icon) ? "" : $"<i class=\""{Icon} me-2\""></i>")}{Title}
                    </h2>
                    {(string.IsNullOrEmpty(Subtitle) ? "" : $"<p class=\"text-muted mb-0\">{Subtitle}</p>")}
                </div>
                <div class=""d-flex gap-2"">
                    {(string.IsNullOrEmpty(BackUrl) ? "" : $@"
                        <a href=""{BackUrl}"" class=""btn btn-outline-secondary"">
                            <i class=""fas fa-arrow-left me-1""></i>Back
                        </a>
                    ")}
                    {(string.IsNullOrEmpty(CreateUrl) ? "" : $@"
                        <a href=""{CreateUrl}"" class=""btn btn-primary"">
                            <i class=""fas fa-plus me-1""></i>{CreateText}
                        </a>
                    ")}
                </div>
            </div>
        ";

        output.Content.SetHtmlContent(content);
    }
}

[HtmlTargetElement("data-table")]
public class DataTableTagHelper : TagHelper
{
    [HtmlAttributeName("id")]
    public string Id { get; set; } = "dataTable";

    [HtmlAttributeName("responsive")]
    public bool Responsive { get; set; } = true;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "table-responsive");

        var content = $@"
            <table id=""{Id}"" class=""table table-striped table-hover"">
                {output.GetChildContentAsync().Result.GetContent()}
            </table>
            <script>
                $(document).ready(function() {{
                    $('#{Id}').DataTable({{
                        responsive: {Responsive.ToString().ToLower()},
                        language: {{
                            search: '<i class=""fas fa-search me-2""></i>Search:',
                            lengthMenu: 'Show _MENU_ entries',
                            info: 'Showing _START_ to _END_ of _TOTAL_ entries',
                            paginate: {{
                                first: '<i class=""fas fa-angle-double-left""></i>',
                                last: '<i class=""fas fa-angle-double-right""></i>',
                                next: '<i class=""fas fa-angle-right""></i>',
                                previous: '<i class=""fas fa-angle-left""></i>'
                            }}
                        }}
                    }});
                }});
            </script>
        ";

        output.Content.SetHtmlContent(content);
    }
}

[HtmlTargetElement("status-badge")]
public class StatusBadgeTagHelper : TagHelper
{
    [HtmlAttributeName("status")]
    public string Status { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        
        var (bgClass, icon) = Status?.ToLower() switch
        {
            "active" => ("bg-success", "check-circle"),
            "inactive" => ("bg-secondary", "times-circle"),
            "pending" => ("bg-warning", "clock"),
            "error" => ("bg-danger", "exclamation-circle"),
            _ => ("bg-info", "info-circle")
        };

        output.Attributes.SetAttribute("class", $"badge {bgClass}");
        
        var content = $@"<i class=""fas fa-{icon} me-1""></i>{Status}";
        output.Content.SetHtmlContent(content);
    }
}

[HtmlTargetElement("action-buttons")]
public class ActionButtonsTagHelper : TagHelper
{
    [HtmlAttributeName("id")]
    public string Id { get; set; }

    [HtmlAttributeName("view-url")]
    public string ViewUrl { get; set; }

    [HtmlAttributeName("edit-url")]
    public string EditUrl { get; set; }

    [HtmlAttributeName("delete-url")]
    public string DeleteUrl { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "btn-group");
        output.Attributes.SetAttribute("role", "group");

        var content = $@"
            {(string.IsNullOrEmpty(ViewUrl) ? "" : $@"
                <a href=""{ViewUrl}"" class=""btn btn-info btn-sm"" title=""View"">
                    <i class=""fas fa-eye""></i>
                </a>
            ")}
            {(string.IsNullOrEmpty(EditUrl) ? "" : $@"
                <a href=""{EditUrl}"" class=""btn btn-primary btn-sm"" title=""Edit"">
                    <i class=""fas fa-edit""></i>
                </a>
            ")}
            {(string.IsNullOrEmpty(DeleteUrl) ? "" : $@"
                <button type=""button"" class=""btn btn-danger btn-sm"" title=""Delete""
                        onclick=""showDeleteConfirmation('{DeleteUrl}')"">
                    <i class=""fas fa-trash""></i>
                </button>
            ")}
        ";

        output.Content.SetHtmlContent(content);
    }
}
