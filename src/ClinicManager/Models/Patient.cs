using System.ComponentModel.DataAnnotations;

namespace ClinicManager.Models;

public class Patient
{
    public int PatientId { get; set; }

    [Required, MaxLength(11), MinLength(11)]
    public string Pesel { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string InsuranceNumber { get; set; } = string.Empty;

    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? PhoneNumber { get; set; }

    public DateOnly? BirthDate { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
