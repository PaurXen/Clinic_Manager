using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models;

public class ProcedurePerformed
{
    public int ProcedurePerformedId { get; set; }

    public int VisitId { get; set; }
    public Visit Visit { get; set; } = null!;

    public int ProcedureId { get; set; }
    public Procedure Procedure { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public decimal ServiceCost { get; set; }

    public ICollection<PrescribedMedication> PrescribedMedications { get; set; } = new List<PrescribedMedication>();
}
