using ClinicManager.DTOs;
using ClinicManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, dto.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded) return Unauthorized("Nieprawidłowy login lub hasło.");
        _logger.LogInformation("User {Email} logged in", dto.Email);
        return Ok(new { message = "Zalogowano." });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Wylogowano." });
    }

    [HttpPost("register")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        if (!new[] { RoleNames.Admin, RoleNames.Doctor, RoleNames.Registrar }.Contains(dto.Role))
        {
            return BadRequest("Nieprawidłowa rola.");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        await _userManager.AddToRoleAsync(user, dto.Role);
        return Created($"/api/users/{user.Id}", new { user.Id, user.Email, dto.Role });
    }

    [HttpGet("access-denied")]
    [AllowAnonymous]
    public IActionResult AccessDenied() => Forbid();
}
