using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

public record MedicationDto(int MedicationId, string Name, string Form, decimal UnitPrice, bool IsActive);
public record CreateMedicationDto([Required, MaxLength(160)] string Name, [MaxLength(120)] string Form, [Range(0,999999)] decimal UnitPrice, bool IsActive = true);
public record UpdateMedicationDto([Required, MaxLength(160)] string Name, [MaxLength(120)] string Form, [Range(0,999999)] decimal UnitPrice, bool IsActive = true);

public record ProcedureDto(int ProcedureId, string Name, string Description, decimal ServiceCost);
public record CreateProcedureDto([Required, MaxLength(160)] string Name, [Required, MaxLength(500)] string Description, [Range(0,999999)] decimal ServiceCost);
public record UpdateProcedureDto([Required, MaxLength(160)] string Name, [Required, MaxLength(500)] string Description, [Range(0,999999)] decimal ServiceCost);
