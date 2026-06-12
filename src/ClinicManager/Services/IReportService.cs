using ClinicManager.DTOs;

namespace ClinicManager.Services;

public interface IReportService
{
    Task<CostReportDto> GetCostReportAsync(int? patientId, string? doctorId, int? month, int? year);
    Task<byte[]> GenerateCostReportPdfAsync(int? patientId, string? doctorId, int? month, int? year);
    Task<byte[]> GenerateUpcomingVisitsPdfAsync(DateOnly date);
}

public interface IPdfReportService
{
    byte[] GenerateCostReport(CostReportDto report);
    byte[] GenerateUpcomingVisitsReport(IEnumerable<VisitListItemDto> visits, DateOnly date);
    byte[] GenerateVisitCard(VisitDetailsDto visit);
    byte[] GeneratePrescription(VisitDetailsDto visit);
}

public interface IFileStorageService
{
    Task<string> SaveMedicalDocumentAsync(IFormFile file, int patientId);
}

public interface IEmailSender
{
    Task SendWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName, CancellationToken cancellationToken);
}
