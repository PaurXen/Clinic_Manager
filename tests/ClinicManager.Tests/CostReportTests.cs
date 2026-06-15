using ClinicManager.DTOs;
using ClinicManager.Services;
using Xunit;

namespace ClinicManager.Tests;

public class CostReportTests
{
    [Fact]
    public void PdfReport_GeneratesNonEmptyPdf()
    {
        var service = new PdfReportService();
        var report = new CostReportDto(150m, new[]
        {
            new CostReportRowDto(1, DateTime.Today, "Jan Kowalski", "Dr Test", 100m, 50m, 150m)
        });

        var pdf = service.GenerateCostReport(report);

        Assert.True(pdf.Length > 1000);
        Assert.Equal('%', (char)pdf[0]);
    }
}
