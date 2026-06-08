using System.ComponentModel.DataAnnotations;

namespace ClinicManager.DTOs;

public record LoginDto([Required, EmailAddress] string Email, [Required] string Password, bool RememberMe = false);

public record RegisterUserDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string Role);
