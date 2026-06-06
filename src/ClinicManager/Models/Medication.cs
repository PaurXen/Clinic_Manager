using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models;

public class Medication
{
    public int MedicationId { get; set; }

    [Required, MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Form { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal UnitPrice { get; set; }

    public bool IsActive { get; set; } = true;
}
