using ClinicManager.Data;
using ClinicManager.Mappers;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Web;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing DefaultConnection.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "ClinicManager.Auth";
    options.LoginPath = "/api/account/login";
    options.AccessDeniedPath = "/api/account/access-denied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StaffOnly", p => p.RequireRole(RoleNames.Admin, RoleNames.Doctor, RoleNames.Registrar));
    options.AddPolicy("AdminOrRegistrar", p => p.RequireRole(RoleNames.Admin, RoleNames.Registrar));
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClinicManager API",
        Version = "v1",
        Description = "API do obsługi przychodni medycznej."
    });
});

builder.Services.AddScoped<PatientMapper>();
builder.Services.AddScoped<VisitMapper>();
builder.Services.AddScoped<MedicationMapper>();
builder.Services.AddScoped<ProcedureMapper>();

builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IMedicationService, MedicationService>();
builder.Services.AddScoped<IProcedureService, ProcedureService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPdfReportService, PdfReportService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

if (builder.Configuration.GetValue<bool>("BackgroundReports:Enabled"))
{
    builder.Services.AddHostedService<UpcomingVisitsReportBackgroundService>();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
