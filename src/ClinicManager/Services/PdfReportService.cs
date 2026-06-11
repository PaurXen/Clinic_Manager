using ClinicManager.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicManager.Services;

public class PdfReportService : IPdfReportService
{
    public byte[] GenerateCostReport(CostReportDto report) => Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Header().Text("Raport kosztów świadczeń").FontSize(20).Bold();
            page.Content().Column(col =>
            {
                col.Item().Text($"Suma: {report.TotalCost:C}").FontSize(14).Bold();
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); c.ConstantColumn(70); c.ConstantColumn(70); c.ConstantColumn(70);
                    });
                    table.Header(h =>
                    {
                        h.Cell().Text("Data").Bold(); h.Cell().Text("Pacjent").Bold(); h.Cell().Text("Lekarz").Bold(); h.Cell().Text("Procedury").Bold(); h.Cell().Text("Leki").Bold(); h.Cell().Text("Razem").Bold();
                    });
                    foreach (var row in report.Rows)
                    {
                        table.Cell().Text(row.VisitDate.ToString("yyyy-MM-dd HH:mm"));
                        table.Cell().Text(row.PatientFullName);
                        table.Cell().Text(row.DoctorFullName ?? "-");
                        table.Cell().Text(row.ProceduresCost.ToString("0.00"));
                        table.Cell().Text(row.MedicationCost.ToString("0.00"));
                        table.Cell().Text(row.TotalCost.ToString("0.00"));
                    }
                });
            });
            page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
        });
    }).GeneratePdf();

    public byte[] GenerateUpcomingVisitsReport(IEnumerable<VisitListItemDto> visits, DateOnly date) => Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Header().Text($"Nadchodzące wizyty - {date:yyyy-MM-dd}").FontSize(20).Bold();
            page.Content().Table(table =>
            {
                table.ColumnsDefinition(c => { c.ConstantColumn(80); c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); });
                table.Header(h => { h.Cell().Text("Godzina").Bold(); h.Cell().Text("Pacjent").Bold(); h.Cell().Text("Lekarz").Bold(); h.Cell().Text("Powód").Bold(); });
                foreach (var v in visits)
                {
                    table.Cell().Text(v.StartAt.ToString("HH:mm"));
                    table.Cell().Text(v.PatientFullName);
                    table.Cell().Text(v.DoctorFullName ?? "-");
                    table.Cell().Text(v.Reason ?? "-");
                }
            });
        });
    }).GeneratePdf();

    public byte[] GenerateVisitCard(VisitDetailsDto visit) => Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Header().Text($"Karta wizyty #{visit.VisitId}").FontSize(20).Bold();
            page.Content().Column(col =>
            {
                col.Item().Text($"Pacjent: {visit.PatientFullName}");
                col.Item().Text($"Lekarz: {visit.DoctorFullName ?? "-"}");
                col.Item().Text($"Termin: {visit.StartAt:yyyy-MM-dd HH:mm} - {visit.EndAt:HH:mm}");
                col.Item().Text($"Status: {visit.Status}");
                col.Item().PaddingTop(10).Text("Procedury").Bold();
                foreach (var p in visit.Procedures)
                    col.Item().Text($"- {p.ProcedureName}: {p.ServiceCost:0.00} zł, {p.Description}");
                col.Item().PaddingTop(10).Text("Notatki kliniczne").Bold();
                foreach (var n in visit.ClinicalNotes)
                    col.Item().Text($"- {n.Timestamp:yyyy-MM-dd HH:mm}: {n.Content} Diagnoza: {n.Diagnosis ?? "-"}; Zalecenia: {n.Recommendations ?? "-"}");
            });
        });
    }).GeneratePdf();

    public byte[] GeneratePrescription(VisitDetailsDto visit) => Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Margin(30);
            page.Header().Text($"Recepta - wizyta #{visit.VisitId}").FontSize(20).Bold();
            page.Content().Column(col =>
            {
                col.Item().Text($"Pacjent: {visit.PatientFullName}");
                col.Item().Text($"Lekarz: {visit.DoctorFullName ?? "-"}");
                col.Item().PaddingTop(10).Text("Leki").Bold();
                foreach (var med in visit.Procedures.SelectMany(p => p.Medications))
                {
                    col.Item().Text($"- {med.MedicationName}, dawkowanie: {med.Dosage}, ilość: {med.Quantity}");
                }
            });
        });
    }).GeneratePdf();
}
