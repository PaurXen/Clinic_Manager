using ClinicManager.DTOs;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mappers;

[Mapper]
public partial class PatientMapper
{
    public PatientDto ToDto(Patient patient) => new(
        patient.PatientId,
        patient.Pesel,
        patient.FirstName,
        patient.LastName,
        patient.InsuranceNumber,
        patient.Email,
        patient.PhoneNumber,
        patient.BirthDate);

    public Patient ToEntity(CreatePatientDto dto) => new()
    {
        Pesel = dto.Pesel,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        InsuranceNumber = dto.InsuranceNumber,
        Email = dto.Email,
        PhoneNumber = dto.PhoneNumber,
        BirthDate = dto.BirthDate
    };

    public void Update(UpdatePatientDto dto, Patient patient)
    {
        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.InsuranceNumber = dto.InsuranceNumber;
        patient.Email = dto.Email;
        patient.PhoneNumber = dto.PhoneNumber;
        patient.BirthDate = dto.BirthDate;
    }

    public MedicalRecordDto ToDto(MedicalRecord record) => new(
        record.MedicalRecordId,
        record.PatientId,
        record.Title,
        record.Description,
        record.DocumentScanUrl,
        record.CreatedAt);
}
