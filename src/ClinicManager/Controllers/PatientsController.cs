using ClinicManager.DTOs;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOnly")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patients;
    private readonly IFileStorageService _files;

    public PatientsController(IPatientService patients, IFileStorageService files)
    {
        _patients = patients;
        _files = files;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientDto>>> GetAll() => Ok(await _patients.GetAllAsync());

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<PatientDto>>> Search([FromQuery] string? query) => Ok(await _patients.SearchAsync(query));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PatientDto>> Get(int id)
    {
        var patient = await _patients.GetByIdAsync(id);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpGet("{id:int}/visits")]
    public async Task<ActionResult<IReadOnlyList<VisitListItemDto>>> GetVisits(int id) => Ok(await _patients.GetVisitsAsync(id));

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Registrar)]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientDto dto)
    {
        var patient = await _patients.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = patient.PatientId }, patient);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Registrar)]
    public async Task<IActionResult> Update(int id, UpdatePatientDto dto) => await _patients.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(int id) => await _patients.SoftDeleteAsync(id) ? NoContent() : NotFound();

    [HttpGet("{patientId:int}/records")]
    public async Task<ActionResult<IReadOnlyList<MedicalRecordDto>>> Records(int patientId) => Ok(await _patients.GetRecordsAsync(patientId));

    [HttpPost("{patientId:int}/records/upload")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Registrar + "," + RoleNames.Doctor)]
    public async Task<ActionResult<MedicalRecordDto>> UploadRecord(int patientId, [FromForm] CreateMedicalRecordDto dto, IFormFile? file)
    {
        var url = file is null ? null : await _files.SaveMedicalDocumentAsync(file, patientId);
        var record = await _patients.AddRecordAsync(patientId, dto, url);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpDelete("{patientId:int}/records/{recordId:int}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> DeleteRecord(int patientId, int recordId) => await _patients.DeleteRecordAsync(patientId, recordId) ? NoContent() : NotFound();
}
