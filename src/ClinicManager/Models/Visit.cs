using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models;

public class Visit
{
    public int VisitId { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public string? AssignedDoctorId { get; set; }
    public ApplicationUser? AssignedDoctor { get; set; }

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }

    public VisitStatus Status { get; set; } = VisitStatus.Planned;

    [MaxLength(500)]
    public string? Reason { get; set; }

    public ICollection<ProcedurePerformed> ProceduresPerformed { get; set; } = new List<ProcedurePerformed>();
    public ICollection<ClinicalNote> ClinicalNotes { get; set; } = new List<ClinicalNote>();
}
