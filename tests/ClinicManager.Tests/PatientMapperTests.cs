using ClinicManager.DTOs;
using ClinicManager.Mappers;
using Xunit;

namespace ClinicManager.Tests;

public class PatientMapperTests
{
    [Fact]
    public void ToEntity_CopiesPeselAndName()
    {
        var mapper = new PatientMapper();
        var dto = new CreatePatientDto("90010112345", "Jan", "Kowalski", "NFZ-1", null, null, null);

        var entity = mapper.ToEntity(dto);

        Assert.Equal("90010112345", entity.Pesel);
        Assert.Equal("Jan", entity.FirstName);
        Assert.Equal("Kowalski", entity.LastName);
    }
}
