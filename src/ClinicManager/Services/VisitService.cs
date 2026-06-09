using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;
using ClinicManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class VisitService : IVisitService
{
    private readonly ApplicationDbContext _db;
    private readonly VisitMapper _mapper;
    private readonly IPdfReportService _pdf;
    private readonly ILogger<VisitService> _logger;

    public VisitService(ApplicationDbContext db, VisitMapper mapper, IPdfReportService pdf, ILogger<VisitService> logger)
    {
        _db = db;
        _mapper = mapper;
        _pdf = pdf;
        _logger = logger;
    }

    public async Task<VisitDetailsDto?> GetByIdAsync(int id)
    {
        var visit = await IncludeFullVisit(_db.Visits.AsNoTracking()).FirstOrDefaultAsync(v => v.VisitId == id);
        return visit is null ? null : _mapper.ToDetails(visit);
    }

    public async Task<VisitDetailsDto> CreateAsync(CreateVisitDto dto)
    {
        if (dto.EndAt <= dto.StartAt)
        {
            throw new InvalidOperationException("Data zakończenia wizyty musi być późniejsza niż data rozpoczęcia.");
        }

        if (!await _db.Patients.AnyAsync(p => p.PatientId == dto.PatientId))
        {
            throw new InvalidOperationException("Pacjent nie istnieje.");
        }

        var visit = _mapper.ToEntity(dto);
        _db.Visits.Add(visit);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Created visit {VisitId}", visit.VisitId);

        return (await GetByIdAsync(visit.VisitId))!;
    }

    public async Task<IReadOnlyList<VisitListItemDto>> GetTodayAsync()
    {
        var start = DateTime.Today;
        var end = start.AddDays(1);
        var visits = await BaseVisitListQuery()
            .Where(v => v.StartAt >= start && v.StartAt < end)
            .OrderBy(v => v.StartAt)
            .ToListAsync();
        return visits.Select(_mapper.ToListItem).ToList();
    }

    public async Task<IReadOnlyList<VisitListItemDto>> GetByDoctorOnDateAsync(string doctorId, DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = start.AddDays(1);
        var visits = await BaseVisitListQuery()
            .Where(v => v.AssignedDoctorId == doctorId && v.StartAt >= start && v.StartAt < end)
            .OrderBy(v => v.StartAt)
            .ToListAsync();
        return visits.Select(_mapper.ToListItem).ToList();
    }

    public async Task<IReadOnlyList<ActiveVisitDto>> GetActiveAsync()
    {
        var visits = await IncludeFullVisit(_db.Visits.AsNoTracking())
            .Where(v => v.Status == VisitStatus.Planned || v.Status == VisitStatus.InProgress)
            .OrderBy(v => v.StartAt)
            .Take(100)
            .ToListAsync();
        return visits.Select(_mapper.ToActiveVisit).ToList();
    }

    public async Task<bool> ChangeStatusAsync(int id, VisitStatus status)
    {
        var visit = await _db.Visits.FirstOrDefaultAsync(v => v.VisitId == id);
        if (visit is null) return false;

        visit.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ProcedurePerformedDto?> AddProcedureAsync(int visitId, AddProcedureToVisitDto dto)
    {
        var visitExists = await _db.Visits.AnyAsync(v => v.VisitId == visitId);
        var procedure = await _db.Procedures.FindAsync(dto.ProcedureId);
        if (!visitExists || procedure is null) return null;

        var performed = new ProcedurePerformed
        {
            VisitId = visitId,
            ProcedureId = procedure.ProcedureId,
            Description = dto.Description ?? procedure.Description,
            ServiceCost = procedure.ServiceCost
        };

        _db.ProceduresPerformed.Add(performed);
        await _db.SaveChangesAsync();

        performed.Procedure = procedure;
        performed.PrescribedMedications = new List<PrescribedMedication>();
        return _mapper.ToProcedurePerformed(performed);
    }

    public async Task<PrescribedMedicationDto?> PrescribeMedicationAsync(int procedurePerformedId, PrescribeMedicationDto dto)
    {
        var performed = await _db.ProceduresPerformed.FindAsync(procedurePerformedId);
        var medication = await _db.Medications.FindAsync(dto.MedicationId);
        if (performed is null || medication is null || !medication.IsActive) return null;

        var prescribed = new PrescribedMedication
        {
            ProcedurePerformedId = procedurePerformedId,
            MedicationId = medication.MedicationId,
            Dosage = dto.Dosage,
            Quantity = dto.Quantity,
            UnitPriceSnapshot = medication.UnitPrice
        };

        _db.PrescribedMedications.Add(prescribed);
        await _db.SaveChangesAsync();

        prescribed.Medication = medication;
        return _mapper.ToPrescribedMedication(prescribed);
    }

    public async Task<ClinicalNoteDto?> AddNoteAsync(int visitId, AddClinicalNoteDto dto, string? authorId)
    {
        if (!await _db.Visits.AnyAsync(v => v.VisitId == visitId)) return null;

        var note = new ClinicalNote
        {
            VisitId = visitId,
            AuthorId = authorId,
            Content = dto.Content,
            Diagnosis = dto.Diagnosis,
            Recommendations = dto.Recommendations,
            Timestamp = DateTime.UtcNow
        };

        _db.ClinicalNotes.Add(note);
        await _db.SaveChangesAsync();

        note.Author = authorId is null ? null : await _db.Users.FindAsync(authorId);
        return _mapper.ToClinicalNote(note);
    }

    public async Task<byte[]?> GenerateVisitCardPdfAsync(int visitId)
    {
        var visit = await GetByIdAsync(visitId);
        return visit is null ? null : _pdf.GenerateVisitCard(visit);
    }

    public async Task<byte[]?> GeneratePrescriptionPdfAsync(int visitId)
    {
        var visit = await GetByIdAsync(visitId);
        return visit is null ? null : _pdf.GeneratePrescription(visit);
    }

    private IQueryable<Visit> BaseVisitListQuery() => _db.Visits.AsNoTracking()
        .Include(v => v.Patient)
        .Include(v => v.AssignedDoctor);

    private static IQueryable<Visit> IncludeFullVisit(IQueryable<Visit> query) => query
        .Include(v => v.Patient)
        .Include(v => v.AssignedDoctor)
        .Include(v => v.ProceduresPerformed)
            .ThenInclude(p => p.Procedure)
        .Include(v => v.ProceduresPerformed)
            .ThenInclude(p => p.PrescribedMedications)
                .ThenInclude(m => m.Medication)
        .Include(v => v.ClinicalNotes)
            .ThenInclude(n => n.Author);
}
