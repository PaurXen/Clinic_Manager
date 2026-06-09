using ClinicManager.DTOs;
using ClinicManager.Models;

namespace ClinicManager.Services;

public interface IVisitService
{
    Task<VisitDetailsDto?> GetByIdAsync(int id);
    Task<VisitDetailsDto> CreateAsync(CreateVisitDto dto);
    Task<IReadOnlyList<VisitListItemDto>> GetTodayAsync();
    Task<IReadOnlyList<VisitListItemDto>> GetByDoctorOnDateAsync(string doctorId, DateOnly date);
    Task<IReadOnlyList<ActiveVisitDto>> GetActiveAsync();
    Task<bool> ChangeStatusAsync(int id, VisitStatus status);
    Task<ProcedurePerformedDto?> AddProcedureAsync(int visitId, AddProcedureToVisitDto dto);
    Task<PrescribedMedicationDto?> PrescribeMedicationAsync(int procedurePerformedId, PrescribeMedicationDto dto);
    Task<ClinicalNoteDto?> AddNoteAsync(int visitId, AddClinicalNoteDto dto, string? authorId);
    Task<byte[]?> GenerateVisitCardPdfAsync(int visitId);
    Task<byte[]?> GeneratePrescriptionPdfAsync(int visitId);
}
