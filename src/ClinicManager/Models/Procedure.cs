using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models;

public class Procedure
{
    public int ProcedureId { get; set; }

    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal ServiceCost { get; set; }

    public ICollection<ProcedurePerformed> ProceduresPerformed { get; set; } = new List<ProcedurePerformed>();
}
