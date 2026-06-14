namespace ClinicManager.Services;

public class UpcomingVisitsReportBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpcomingVisitsReportBackgroundService> _logger;

    public UpcomingVisitsReportBackgroundService(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<UpcomingVisitsReportBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = Math.Max(1, _configuration.GetValue<int>("BackgroundReports:IntervalMinutes", 1440));
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(intervalMinutes));

        await GenerateAndSendAsync(stoppingToken);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await GenerateAndSendAsync(stoppingToken);
        }
    }

    private async Task GenerateAndSendAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var reports = scope.ServiceProvider.GetRequiredService<IReportService>();
            var email = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            var pdf = await reports.GenerateUpcomingVisitsPdfAsync(tomorrow);

            var outputDir = _configuration["BackgroundReports:OutputDirectory"] ?? "reports";
            Directory.CreateDirectory(outputDir);
            var path = Path.Combine(outputDir, "upcoming_visits.pdf");
            await File.WriteAllBytesAsync(path, pdf, cancellationToken);

            var adminEmail = _configuration["BackgroundReports:AdminEmail"] ?? "admin@clinic.local";
            await email.SendWithAttachmentAsync(adminEmail, "Raport wizyt na jutro", "W załączniku raport PDF.", pdf, "upcoming_visits.pdf", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate or send upcoming visits report.");
        }
    }
}
