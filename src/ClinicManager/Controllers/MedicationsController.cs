using ClinicManager.DTOs;
using ClinicManager.Models;
using ClinicManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOnly")]
public class MedicationsController : ControllerBase
{
    private readonly IMedicationService _service;
    public MedicationsController(IMedicationService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MedicationDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MedicationDto>> Get(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Registrar)]
    public async Task<ActionResult<MedicationDto>> Create(CreateMedicationDto dto)
    {
        var item = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = item.MedicationId }, item);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Registrar)]
    public async Task<IActionResult> Update(int id, UpdateMedicationDto dto) => await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Registrar)]
    public async Task<IActionResult> Delete(int id) => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
