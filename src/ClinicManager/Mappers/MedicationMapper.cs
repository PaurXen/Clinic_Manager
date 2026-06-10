using ClinicManager.DTOs;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mappers;

[Mapper]
public partial class MedicationMapper
{
    public MedicationDto ToDto(Medication medication) => new(medication.MedicationId, medication.Name, medication.Form, medication.UnitPrice, medication.IsActive);

    public Medication ToEntity(CreateMedicationDto dto) => new()
    {
        Name = dto.Name,
        Form = dto.Form,
        UnitPrice = dto.UnitPrice,
        IsActive = dto.IsActive
    };

    public void Update(UpdateMedicationDto dto, Medication medication)
    {
        medication.Name = dto.Name;
        medication.Form = dto.Form;
        medication.UnitPrice = dto.UnitPrice;
        medication.IsActive = dto.IsActive;
    }
}
