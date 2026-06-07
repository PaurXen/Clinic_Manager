# Migracje EF Core

W tym środowisku nie ma zainstalowanego SDK `dotnet`, więc migracja nie została wygenerowana automatycznie.
Po rozpakowaniu projektu uruchom lokalnie:

```bash
cd src/ClinicManager
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Model zawiera konfigurację tabel, relacji i indeksów w `ApplicationDbContext.cs`, a dane startowe w `DbInitializer.cs`.
