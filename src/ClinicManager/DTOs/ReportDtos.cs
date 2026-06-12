namespace ClinicManager.DTOs;

public record CostReportRowDto(int VisitId, DateTime VisitDate, string PatientFullName, string? DoctorFullName, decimal ProceduresCost, decimal MedicationCost, decimal TotalCost);
public record CostReportDto(decimal TotalCost, IReadOnlyList<CostReportRowDto> Rows);
