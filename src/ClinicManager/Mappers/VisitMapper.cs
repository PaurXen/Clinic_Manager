using ClinicManager.DTOs;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mappers;

[Mapper]
public partial class VisitMapper
{
    public VisitListItemDto ToListItem(Visit visit) => new(
        visit.VisitId,
        visit.PatientId,
        $"{visit.Patient.FirstName} {visit.Patient.LastName}".Trim(),
        visit.AssignedDoctorId,
        visit.AssignedDoctor is null ? null : visit.AssignedDoctor.FullName,
        visit.StartAt,
        visit.EndAt,
        visit.Status,
        visit.Reason);

    public ActiveVisitDto ToActiveVisit(Visit visit) => new(
        visit.VisitId,
        $"{visit.Patient.FirstName} {visit.Patient.LastName}".Trim(),
        visit.Patient.Pesel,
        visit.AssignedDoctor is null ? null : visit.AssignedDoctor.FullName,
        visit.StartAt,
        visit.Status,
        visit.ProceduresPerformed.Sum(p => p.ServiceCost + p.PrescribedMedications.Sum(m => m.UnitPriceSnapshot * m.Quantity)));

    public Visit ToEntity(CreateVisitDto dto) => new()
    {
        PatientId = dto.PatientId,
        AssignedDoctorId = dto.AssignedDoctorId,
        StartAt = dto.StartAt,
        EndAt = dto.EndAt,
        Reason = dto.Reason,
        Status = VisitStatus.Planned
    };

    public VisitDetailsDto ToDetails(Visit visit) => new(
        visit.VisitId,
        visit.PatientId,
        $"{visit.Patient.FirstName} {visit.Patient.LastName}".Trim(),
        visit.AssignedDoctorId,
        visit.AssignedDoctor is null ? null : visit.AssignedDoctor.FullName,
        visit.StartAt,
        visit.EndAt,
        visit.Status,
        visit.Reason,
        visit.ProceduresPerformed.Select(ToProcedurePerformed).ToList(),
        visit.ClinicalNotes.OrderByDescending(n => n.Timestamp).Select(ToClinicalNote).ToList());

    public ProcedurePerformedDto ToProcedurePerformed(ProcedurePerformed performed) => new(
        performed.ProcedurePerformedId,
        performed.ProcedureId,
        performed.Procedure.Name,
        performed.Description,
        performed.ServiceCost,
        performed.PrescribedMedications.Select(ToPrescribedMedication).ToList());

    public PrescribedMedicationDto ToPrescribedMedication(PrescribedMedication prescribed) => new(
        prescribed.PrescribedMedicationId,
        prescribed.MedicationId,
        prescribed.Medication.Name,
        prescribed.Dosage,
        prescribed.Quantity,
        prescribed.UnitPriceSnapshot,
        prescribed.UnitPriceSnapshot * prescribed.Quantity);

    public ClinicalNoteDto ToClinicalNote(ClinicalNote note) => new(
        note.ClinicalNoteId,
        note.Author is null ? null : note.Author.FullName,
        note.Content,
        note.Diagnosis,
        note.Recommendations,
        note.Timestamp);
}
