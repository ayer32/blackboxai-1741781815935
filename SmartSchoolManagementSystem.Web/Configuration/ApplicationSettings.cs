namespace SmartSchoolManagementSystem.Web.Configuration;

public class ApplicationSettings
{
    public int PageSize { get; set; } = 10;
    public int MaxBookLendingDays { get; set; } = 14;
    public int MaxActiveLoansPerStudent { get; set; } = 3;
    public decimal DefaultLateFeePerDay { get; set; } = 1.00m;
    public AttendanceSettings AttendanceSettings { get; set; } = new();
    public EmailSettings EmailSettings { get; set; } = new();
}

public class AttendanceSettings
{
    public int AutoMarkAbsentAfterHours { get; set; } = 2;
    public int MinimumAttendancePercentage { get; set; } = 75;
}

public class EmailSettings
{
    public string SendGridApiKey { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
}

public static class ApplicationSettingsExtensions
{
    public static IServiceCollection AddApplicationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ApplicationSettings>(
            configuration.GetSection("ApplicationSettings"));

        return services;
    }
}
