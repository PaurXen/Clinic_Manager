using ClinicManager.DTOs;

namespace ClinicManager.Services;

public interface IMedicationService
{
    Task<IReadOnlyList<MedicationDto>> GetAllAsync();
    Task<MedicationDto?> GetByIdAsync(int id);
    Task<MedicationDto> CreateAsync(CreateMedicationDto dto);
    Task<bool> UpdateAsync(int id, UpdateMedicationDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IProcedureService
{
    Task<IReadOnlyList<ProcedureDto>> GetAllAsync();
    Task<ProcedureDto?> GetByIdAsync(int id);
    Task<ProcedureDto> CreateAsync(CreateProcedureDto dto);
    Task<bool> UpdateAsync(int id, UpdateProcedureDto dto);
    Task<bool> DeleteAsync(int id);
}
