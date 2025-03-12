using SmartSchoolManagementSystem.Web.Configuration;
using SmartSchoolManagementSystem.Web.HealthChecks;
using SmartSchoolManagementSystem.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.ConfigureLogging();

// Add services to the container
builder.Services
    .AddCustomLogging()
    .AddCustomHealthChecks()
    .AddCustomCors(builder.Configuration)
    .AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use CORS before auth
app.UseCors("DefaultPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Use custom middleware
app.UseApplicationMiddleware();

// Use health checks
app.UseCustomHealthChecks();

app.MapRazorPages();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}

// Log application startup
Log.Information("Starting up Smart School Management System...");

app.Run();
