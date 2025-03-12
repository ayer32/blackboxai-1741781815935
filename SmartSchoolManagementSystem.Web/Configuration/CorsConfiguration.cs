namespace SmartSchoolManagementSystem.Web.Configuration;

public static class CorsConfiguration
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection("CorsSettings").Get<CorsSettings>() ?? new CorsSettings();

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", builder =>
            {
                builder
                    .WithOrigins(corsSettings.AllowedOrigins ?? Array.Empty<string>())
                    .WithMethods(corsSettings.AllowedMethods ?? Array.Empty<string>())
                    .WithHeaders(corsSettings.AllowedHeaders ?? Array.Empty<string>())
                    .WithExposedHeaders(corsSettings.ExposedHeaders ?? Array.Empty<string>())
                    .SetIsOriginAllowedToAllowWildcardSubdomains();

                if (corsSettings.AllowCredentials)
                {
                    builder.AllowCredentials();
                }
                else
                {
                    builder.DisallowCredentials();
                }

                if (corsSettings.AllowAnyOrigin)
                {
                    builder.SetIsOriginAllowed(_ => true);
                }

                if (corsSettings.AllowAnyMethod)
                {
                    builder.AllowAnyMethod();
                }

                if (corsSettings.AllowAnyHeader)
                {
                    builder.AllowAnyHeader();
                }
            });

            // Add additional policies if needed
            options.AddPolicy("ApiPolicy", builder =>
            {
                builder
                    .WithOrigins("https://api.smartschool.com")
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; }
    public string[] AllowedMethods { get; set; }
    public string[] AllowedHeaders { get; set; }
    public string[] ExposedHeaders { get; set; }
    public bool AllowCredentials { get; set; }
    public bool AllowAnyOrigin { get; set; }
    public bool AllowAnyMethod { get; set; }
    public bool AllowAnyHeader { get; set; }
}
