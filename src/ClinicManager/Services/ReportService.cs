using ClinicManager.Data;
using ClinicManager.DTOs;
using ClinicManager.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _db;
    private readonly IPdfReportService _pdf;
    private readonly VisitMapper _visitMapper;

    public ReportService(ApplicationDbContext db, IPdfReportService pdf, VisitMapper visitMapper)
    {
        _db = db;
        _pdf = pdf;
        _visitMapper = visitMapper;
    }

    public async Task<CostReportDto> GetCostReportAsync(int? patientId, string? doctorId, int? month, int? year)
    {
        var query = _db.Visits.AsNoTracking()
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Include(v => v.ProceduresPerformed)
                .ThenInclude(p => p.PrescribedMedications)
            .AsQueryable();

        if (patientId.HasValue) query = query.Where(v => v.PatientId == patientId.Value);
        if (!string.IsNullOrWhiteSpace(doctorId)) query = query.Where(v => v.AssignedDoctorId == doctorId);
        if (month.HasValue) query = query.Where(v => v.StartAt.Month == month.Value);
        if (year.HasValue) query = query.Where(v => v.StartAt.Year == year.Value);

        var visits = await query.OrderByDescending(v => v.StartAt).ToListAsync();
        var rows = visits.Select(v =>
        {
            var procCost = v.ProceduresPerformed.Sum(p => p.ServiceCost);
            var medCost = v.ProceduresPerformed.Sum(p => p.PrescribedMedications.Sum(m => m.UnitPriceSnapshot * m.Quantity));
            return new CostReportRowDto(
                v.VisitId,
                v.StartAt,
                $"{v.Patient.FirstName} {v.Patient.LastName}".Trim(),
                v.AssignedDoctor?.FullName,
                procCost,
                medCost,
                procCost + medCost);
        }).ToList();

        return new CostReportDto(rows.Sum(r => r.TotalCost), rows);
    }

    public async Task<byte[]> GenerateCostReportPdfAsync(int? patientId, string? doctorId, int? month, int? year)
    {
        var report = await GetCostReportAsync(patientId, doctorId, month, year);
        return _pdf.GenerateCostReport(report);
    }

    public async Task<byte[]> GenerateUpcomingVisitsPdfAsync(DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = start.AddDays(1);
        var visits = await _db.Visits.AsNoTracking()
            .Include(v => v.Patient)
            .Include(v => v.AssignedDoctor)
            .Where(v => v.StartAt >= start && v.StartAt < end && v.Status == Models.VisitStatus.Planned)
            .OrderBy(v => v.StartAt)
            .ToListAsync();

        return _pdf.GenerateUpcomingVisitsReport(visits.Select(_visitMapper.ToListItem), date);
    }
}
