using ClinicManager.DTOs;

namespace ClinicManager.Services;

public interface IPatientService
{
    Task<IReadOnlyList<PatientDto>> GetAllAsync();
    Task<PatientDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<PatientDto>> SearchAsync(string? query);
    Task<IReadOnlyList<MedicalRecordDto>> GetRecordsAsync(int patientId);
    Task<IReadOnlyList<VisitListItemDto>> GetVisitsAsync(int patientId);
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
    Task<bool> UpdateAsync(int id, UpdatePatientDto dto);
    Task<bool> SoftDeleteAsync(int id);
    Task<MedicalRecordDto?> AddRecordAsync(int patientId, CreateMedicalRecordDto dto, string? documentUrl);
    Task<bool> DeleteRecordAsync(int patientId, int recordId);
}
