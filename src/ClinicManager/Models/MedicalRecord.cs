using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models;

public class MedicalRecord
{
    public int MedicalRecordId { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [Required, MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(260)]
    public string? DocumentScanUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
