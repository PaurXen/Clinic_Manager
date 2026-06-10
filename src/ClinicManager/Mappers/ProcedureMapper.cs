using ClinicManager.DTOs;
using ClinicManager.Models;
using Riok.Mapperly.Abstractions;

namespace ClinicManager.Mappers;

[Mapper]
public partial class ProcedureMapper
{
    public ProcedureDto ToDto(Procedure procedure) => new(procedure.ProcedureId, procedure.Name, procedure.Description, procedure.ServiceCost);

    public Procedure ToEntity(CreateProcedureDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        ServiceCost = dto.ServiceCost
    };

    public void Update(UpdateProcedureDto dto, Procedure procedure)
    {
        procedure.Name = dto.Name;
        procedure.Description = dto.Description;
        procedure.ServiceCost = dto.ServiceCost;
    }
}
