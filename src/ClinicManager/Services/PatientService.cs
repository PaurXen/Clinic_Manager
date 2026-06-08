using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _db;
    private readonly PatientMapper _mapper;
    private readonly ILogger<PatientService> _logger;
    private readonly VisitMapper _visitMapper;

    public PatientService(ApplicationDbContext db, PatientMapper mapper, VisitMapper visitMapper, ILogger<PatientService> logger)
    {
        _db = db;
        _mapper = mapper;
        _visitMapper = visitMapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PatientDto>> GetAllAsync()
    {
        var patients = await _db.Patients.AsNoTracking()
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .ToListAsync();
        return patients.Select(_mapper.ToDto).ToList();
    }

    public async Task<PatientDto?> GetByIdAsync(int id)
    {
        var patient = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.PatientId == id);
        return patient is null ? null : _mapper.ToDto(patient);
    }

    public async Task<IReadOnlyList<PatientDto>> SearchAsync(string? query)
    {
        query = query?.Trim();
        var patients = _db.Patients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            patients = patients.Where(p => p.Pesel.Contains(query) || p.LastName.Contains(query) || p.FirstName.Contains(query));
        }

        var list = await patients.OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Take(50)
            .ToListAsync();
        return list.Select(_mapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<MedicalRecordDto>> GetRecordsAsync(int patientId)
    {
        var records = await _db.MedicalRecords.AsNoTracking()
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return records.Select(_mapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<VisitListItemDto>> GetVisitsAsync(int patientId)
    {
        var visits = await _db.Visits.AsNoTracking()
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Where(v => v.PatientId == patientId)
            .OrderByDescending(v => v.StartAt)
            .ToListAsync();
        return visits.Select(_visitMapper.ToListItem).ToList();
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto dto)
    {
        if (await _db.Patients.IgnoreQueryFilters().AnyAsync(p => p.Pesel == dto.Pesel))
        {
            throw new InvalidOperationException("Pacjent z takim numerem PESEL już istnieje.");
        }

        var patient = _mapper.ToEntity(dto);
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Created patient {PatientId}", patient.PatientId);
        return _mapper.ToDto(patient);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePatientDto dto)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
        if (patient is null) return false;

        _mapper.Update(dto, patient);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
        if (patient is null) return false;

        patient.IsDeleted = true;
        await _db.SaveChangesAsync();
        _logger.LogInformation("Soft deleted patient {PatientId}", id);
        return true;
    }

    public async Task<MedicalRecordDto?> AddRecordAsync(int patientId, CreateMedicalRecordDto dto, string? documentUrl)
    {
        if (!await _db.Patients.AnyAsync(p => p.PatientId == patientId)) return null;

        var record = new MedicalRecord
        {
            PatientId = patientId,
            Title = dto.Title,
            Description = dto.Description,
            DocumentScanUrl = documentUrl,
            CreatedAt = DateTime.UtcNow
        };

        _db.MedicalRecords.Add(record);
        await _db.SaveChangesAsync();
        return _mapper.ToDto(record);
    }

    public async Task<bool> DeleteRecordAsync(int patientId, int recordId)
    {
        var record = await _db.MedicalRecords.FirstOrDefaultAsync(r => r.PatientId == patientId && r.MedicalRecordId == recordId);
        if (record is null) return false;

        _db.MedicalRecords.Remove(record);
        await _db.SaveChangesAsync();
        return true;
    }
}
