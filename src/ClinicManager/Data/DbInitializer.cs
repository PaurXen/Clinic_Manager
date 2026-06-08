using ClinicManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        try
        {
            if (db.Database.IsRelational())
            {
                var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await db.Database.MigrateAsync();
                }
                else
                {
                    await db.Database.EnsureCreatedAsync();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database migration failed. Run dotnet ef database update locally if needed.");
        }

        foreach (var role in new[] { RoleNames.Admin, RoleNames.Doctor, RoleNames.Registrar })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureUserAsync(userManager, "admin@clinic.local", "Admin", "System", RoleNames.Admin);
        var doctor = await EnsureUserAsync(userManager, "doctor@clinic.local", "Jan", "Kowalski", RoleNames.Doctor);
        await EnsureUserAsync(userManager, "registration@clinic.local", "Anna", "Nowak", RoleNames.Registrar);

        if (!await db.Patients.AnyAsync())
        {
            var patient = new Patient
            {
                Pesel = "90010112345",
                FirstName = "Kacper",
                LastName = "Testowy",
                InsuranceNumber = "NFZ-123456",
                Email = "pacjent@example.com",
                PhoneNumber = "500600700",
                BirthDate = new DateOnly(1990, 1, 1)
            };

            db.Patients.Add(patient);
            db.Procedures.AddRange(
                new Procedure { Name = "Konsultacja internistyczna", Description = "Standardowa konsultacja lekarska", ServiceCost = 150m },
                new Procedure { Name = "Badanie EKG", Description = "Elektrokardiografia spoczynkowa", ServiceCost = 80m });
            db.Medications.AddRange(
                new Medication { Name = "Paracetamol", Form = "tabletki", UnitPrice = 12.50m },
                new Medication { Name = "Amoksycylina", Form = "kapsułki", UnitPrice = 28.90m });
            await db.SaveChangesAsync();

            db.Visits.Add(new Visit
            {
                PatientId = patient.PatientId,
                AssignedDoctorId = doctor.Id,
                StartAt = DateTime.Today.AddHours(10),
                EndAt = DateTime.Today.AddHours(10).AddMinutes(30),
                Status = VisitStatus.Planned,
                Reason = "Kontrola ogólna"
            });
            await db.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> EnsureUserAsync(UserManager<ApplicationUser> userManager, string email, string firstName, string lastName, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, "Pass123!");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }
}
