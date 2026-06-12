using ClinicManager.DTOs;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOnly")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;
    public ReportsController(IReportService reports) => _reports = reports;

    [HttpGet("costs")]
    public async Task<IActionResult> Costs([FromQuery] int? patientId, [FromQuery] string? doctorId, [FromQuery] int? month, [FromQuery] int? year, [FromQuery] string format = "json")
    {
        if (format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
        {
            var pdf = await _reports.GenerateCostReportPdfAsync(patientId, doctorId, month, year);
            return File(pdf, "application/pdf", "cost-report.pdf");
        }

        var report = await _reports.GetCostReportAsync(patientId, doctorId, month, year);
        return Ok(report);
    }

    [HttpGet("upcoming-visits.pdf")]
    public async Task<IActionResult> UpcomingVisits([FromQuery] DateOnly? date)
    {
        var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var pdf = await _reports.GenerateUpcomingVisitsPdfAsync(targetDate);
        return File(pdf, "application/pdf", "raport-nadchodzace-wizyty.pdf");
    }
}
