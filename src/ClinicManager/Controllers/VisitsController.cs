using ClinicManager.DTOs;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOnly")]
public class VisitsController : ControllerBase
{
    private readonly IVisitService _visits;
    private readonly UserManager<ApplicationUser> _userManager;

    public VisitsController(IVisitService visits, UserManager<ApplicationUser> userManager)
    {
        _visits = visits;
        _userManager = userManager;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VisitDetailsDto>> Get(int id)
    {
        var visit = await _visits.GetByIdAsync(id);
        return visit is null ? NotFound() : Ok(visit);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Registrar)]
    public async Task<ActionResult<VisitDetailsDto>> Create(CreateVisitDto dto)
    {
        var visit = await _visits.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = visit.VisitId }, visit);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, ChangeVisitStatusDto dto) => await _visits.ChangeStatusAsync(id, dto.Status) ? NoContent() : NotFound();

    [HttpGet("today")]
    public async Task<ActionResult<IReadOnlyList<VisitListItemDto>>> Today() => Ok(await _visits.GetTodayAsync());

    [HttpGet("doctor/{doctorId}/date/{date}")]
    public async Task<ActionResult<IReadOnlyList<VisitListItemDto>>> ByDoctor(string doctorId, DateOnly date) => Ok(await _visits.GetByDoctorOnDateAsync(doctorId, date));

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ActiveVisitDto>>> Active() => Ok(await _visits.GetActiveAsync());

    [HttpPost("{visitId:int}/procedures")]
    public async Task<ActionResult<ProcedurePerformedDto>> AddProcedure(int visitId, AddProcedureToVisitDto dto)
    {
        var result = await _visits.AddProcedureAsync(visitId, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("procedures-performed/{procedurePerformedId:int}/medications")]
    public async Task<ActionResult<PrescribedMedicationDto>> PrescribeMedication(int procedurePerformedId, PrescribeMedicationDto dto)
    {
        var result = await _visits.PrescribeMedicationAsync(procedurePerformedId, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{visitId:int}/notes")]
    [Authorize(Roles = RoleNames.Doctor + "," + RoleNames.Admin)]
    public async Task<ActionResult<ClinicalNoteDto>> AddNote(int visitId, AddClinicalNoteDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        var result = await _visits.AddNoteAsync(visitId, dto, user?.Id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:int}/card.pdf")]
    public async Task<IActionResult> VisitCard(int id)
    {
        var pdf = await _visits.GenerateVisitCardPdfAsync(id);
        return pdf is null ? NotFound() : File(pdf, "application/pdf", $"visit-{id}-card.pdf");
    }

    [HttpGet("{id:int}/prescription.pdf")]
    public async Task<IActionResult> Prescription(int id)
    {
        var pdf = await _visits.GeneratePrescriptionPdfAsync(id);
        return pdf is null ? NotFound() : File(pdf, "application/pdf", $"visit-{id}-prescription.pdf");
    }
}
