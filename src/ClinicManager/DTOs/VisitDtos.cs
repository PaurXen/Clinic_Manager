using ClinicManager.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

public record VisitListItemDto(int VisitId, int PatientId, string PatientFullName, string? DoctorId, string? DoctorFullName, DateTime StartAt, DateTime EndAt, VisitStatus Status, string? Reason);

public record ActiveVisitDto(int VisitId, string PatientFullName, string Pesel, string? DoctorFullName, DateTime StartAt, VisitStatus Status, decimal CurrentCost);

public record CreateVisitDto(
    [Required] int PatientId,
    string? AssignedDoctorId,
    [Required] DateTime StartAt,
    [Required] DateTime EndAt,
    [MaxLength(500)] string? Reason);

public record ChangeVisitStatusDto([Required] VisitStatus Status);

public record VisitDetailsDto(
    int VisitId,
    int PatientId,
    string PatientFullName,
    string? DoctorId,
    string? DoctorFullName,
    DateTime StartAt,
    DateTime EndAt,
    VisitStatus Status,
    string? Reason,
    IReadOnlyList<ProcedurePerformedDto> Procedures,
    IReadOnlyList<ClinicalNoteDto> ClinicalNotes);

public record AddProcedureToVisitDto([Required] int ProcedureId, [MaxLength(500)] string? Description);

public record ProcedurePerformedDto(int ProcedurePerformedId, int ProcedureId, string ProcedureName, string? Description, decimal ServiceCost, IReadOnlyList<PrescribedMedicationDto> Medications);

public record PrescribeMedicationDto([Required] int MedicationId, [Required, MaxLength(200)] string Dosage, [Range(1,1000)] int Quantity);

public record PrescribedMedicationDto(int PrescribedMedicationId, int MedicationId, string MedicationName, string Dosage, int Quantity, decimal UnitPriceSnapshot, decimal TotalCost);

public record AddClinicalNoteDto([Required, MaxLength(3000)] string Content, [MaxLength(1000)] string? Diagnosis, [MaxLength(1000)] string? Recommendations);

public record ClinicalNoteDto(int ClinicalNoteId, string? AuthorFullName, string Content, string? Diagnosis, string? Recommendations, DateTime Timestamp);
