using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

public record PatientDto(int PatientId, string Pesel, string FirstName, string LastName, string InsuranceNumber, string? Email, string? PhoneNumber, DateOnly? BirthDate);

public record CreatePatientDto(
    [Required, MinLength(11), MaxLength(11)] string Pesel,
    [Required, MaxLength(80)] string FirstName,
    [Required, MaxLength(80)] string LastName,
    [MaxLength(50)] string InsuranceNumber,
    [EmailAddress] string? Email,
    [MaxLength(30)] string? PhoneNumber,
    DateOnly? BirthDate);

public record UpdatePatientDto(
    [Required, MaxLength(80)] string FirstName,
    [Required, MaxLength(80)] string LastName,
    [MaxLength(50)] string InsuranceNumber,
    [EmailAddress] string? Email,
    [MaxLength(30)] string? PhoneNumber,
    DateOnly? BirthDate);

public record MedicalRecordDto(int MedicalRecordId, int PatientId, string Title, string? Description, string? DocumentScanUrl, DateTime CreatedAt);

public record CreateMedicalRecordDto([Required, MaxLength(120)] string Title, [MaxLength(500)] string? Description);
