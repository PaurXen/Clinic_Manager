using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models;

public class PrescribedMedication
{
    public int PrescribedMedicationId { get; set; }

    public int ProcedurePerformedId { get; set; }
    public ProcedurePerformed ProcedurePerformed { get; set; } = null!;

    public int MedicationId { get; set; }
    public Medication Medication { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Dosage { get; set; } = string.Empty;

    [Range(1, 1000)]
    public int Quantity { get; set; }

    public decimal UnitPriceSnapshot { get; set; }
}
