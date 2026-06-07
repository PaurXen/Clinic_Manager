using ClinicManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinicManager.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Procedure> Procedures => Set<Procedure>();
    public DbSet<ProcedurePerformed> ProceduresPerformed => Set<ProcedurePerformed>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<PrescribedMedication> PrescribedMedications => Set<PrescribedMedication>();
    public DbSet<ClinicalNote> ClinicalNotes => Set<ClinicalNote>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Patient>().HasIndex(p => p.Pesel).IsUnique().HasDatabaseName("IX_Patients_Pesel");
        builder.Entity<Patient>().HasIndex(p => new { p.LastName, p.FirstName }).HasDatabaseName("IX_Patients_LastName_FirstName");

        builder.Entity<Visit>().HasIndex(v => new { v.AssignedDoctorId, v.StartAt }).HasDatabaseName("IX_Visits_Doctor_StartAt");
        builder.Entity<Visit>().HasIndex(v => new { v.Status, v.StartAt }).HasDatabaseName("IX_Visits_Status_StartAt");
        builder.Entity<Visit>()
            .HasOne(v => v.AssignedDoctor)
            .WithMany()
            .HasForeignKey(v => v.AssignedDoctorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MedicalRecord>().HasIndex(r => r.PatientId).HasDatabaseName("IX_MedicalRecords_PatientId");
        builder.Entity<ProcedurePerformed>().HasIndex(p => p.VisitId).HasDatabaseName("IX_ProceduresPerformed_VisitId");
        builder.Entity<ClinicalNote>().HasIndex(n => n.VisitId).HasDatabaseName("IX_ClinicalNotes_VisitId");

        builder.Entity<Procedure>().Property(p => p.ServiceCost).HasColumnType("decimal(18,2)");
        builder.Entity<ProcedurePerformed>().Property(p => p.ServiceCost).HasColumnType("decimal(18,2)");
        builder.Entity<Medication>().Property(m => m.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Entity<PrescribedMedication>().Property(m => m.UnitPriceSnapshot).HasColumnType("decimal(18,2)");
    }
}
