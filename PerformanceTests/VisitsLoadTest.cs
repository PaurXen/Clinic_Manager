using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

var baseUrl = args.FirstOrDefault(a => a.StartsWith("--baseUrl="))?.Split('=', 2)[1] ?? "http://localhost:5000";
using var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

var scenario = Scenario.Create("active_visits_100_requests", async context =>
{
    var request = Http.CreateRequest("GET", "/api/visits/active")
        .WithHeader("Accept", "application/json");

    var response = await Http.Send(httpClient, request);
    return response;
})
.WithLoadSimulations(
    Simulation.Inject(rate: 50, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(2))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportFileName("nbomber-report")
    .WithReportFolder("reports")
    .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
    .Run();
