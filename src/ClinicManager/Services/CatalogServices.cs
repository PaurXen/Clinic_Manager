using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class MedicationService : IMedicationService
{
    private readonly ApplicationDbContext _db;
    private readonly MedicationMapper _mapper;

    public MedicationService(ApplicationDbContext db, MedicationMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MedicationDto>> GetAllAsync()
    {
        var items = await _db.Medications.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
        return items.Select(_mapper.ToDto).ToList();
    }
    public async Task<MedicationDto?> GetByIdAsync(int id)
    {
        var item = await _db.Medications.AsNoTracking().FirstOrDefaultAsync(m => m.MedicationId == id);
        return item is null ? null : _mapper.ToDto(item);
    }

    public async Task<MedicationDto> CreateAsync(CreateMedicationDto dto)
    {
        var medication = _mapper.ToEntity(dto);
        _db.Medications.Add(medication);
        await _db.SaveChangesAsync();
        return _mapper.ToDto(medication);
    }

    public async Task<bool> UpdateAsync(int id, UpdateMedicationDto dto)
    {
        var medication = await _db.Medications.FindAsync(id);
        if (medication is null) return false;
        _mapper.Update(dto, medication);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var medication = await _db.Medications.FindAsync(id);
        if (medication is null) return false;
        medication.IsActive = false;
        await _db.SaveChangesAsync();
        return true;
    }
}

public class ProcedureService : IProcedureService
{
    private readonly ApplicationDbContext _db;
    private readonly ProcedureMapper _mapper;

    public ProcedureService(ApplicationDbContext db, ProcedureMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProcedureDto>> GetAllAsync()
    {
        var items = await _db.Procedures.AsNoTracking().OrderBy(p => p.Name).ToListAsync();
        return items.Select(_mapper.ToDto).ToList();
    }
    public async Task<ProcedureDto?> GetByIdAsync(int id)
    {
        var item = await _db.Procedures.AsNoTracking().FirstOrDefaultAsync(p => p.ProcedureId == id);
        return item is null ? null : _mapper.ToDto(item);
    }

    public async Task<ProcedureDto> CreateAsync(CreateProcedureDto dto)
    {
        var procedure = _mapper.ToEntity(dto);
        _db.Procedures.Add(procedure);
        await _db.SaveChangesAsync();
        return _mapper.ToDto(procedure);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProcedureDto dto)
    {
        var procedure = await _db.Procedures.FindAsync(id);
        if (procedure is null) return false;
        _mapper.Update(dto, procedure);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var procedure = await _db.Procedures.FindAsync(id);
        if (procedure is null) return false;
        _db.Procedures.Remove(procedure);
        await _db.SaveChangesAsync();
        return true;
    }
}
