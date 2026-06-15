# ClinicManager - System zarządzania przychodnią medyczną 2.0

Projekt realizuje wymagania z pliku `Readme.md`: ASP.NET Core 10, EF Core Code First, SQL Server, Identity z rolami, Mapperly, OpenAPI, upload skanów, PDF, NLog, BackgroundService, GitHub Actions oraz test wydajnościowy NBomber.

## Stack

- .NET 10 / ASP.NET Core MVC + API
- EF Core + SQL Server
- ASP.NET Identity: role `Admin`, `Lekarz`, `Rejestratorka`
- Mapperly jako warstwa mapperów DTO
- NLog do logowania błędów do `/logs/errors.log`
- QuestPDF do generowania PDF
- BackgroundService do raportu wizyt na następny dzień
- NBomber do testu `GET /api/visits/active`
- xUnit jako przykładowe testy jednostkowe

## Uruchomienie lokalne

1. Uruchom SQL Server, np. z Docker Compose:

```bash
docker compose up -d sqlserver
```

2. Uzupełnij connection string w `src/ClinicManager/appsettings.Development.json`, jeśli port lub hasło są inne.

3. Przygotuj bazę:

```bash
cd src/ClinicManager
dotnet tool install --global dotnet-ef
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

4. OpenAPI/Swagger:

```text
https://localhost:5001/swagger
http://localhost:5000/swagger
```

## Konta seedowane

Po pierwszym uruchomieniu aplikacja tworzy role i konta testowe:

| Rola | Email | Hasło |
|---|---|---|
| Admin | admin@clinic.local | Pass123! |
| Lekarz | doctor@clinic.local | Pass123! |
| Rejestratorka | registration@clinic.local | Pass123! |

Logowanie jest realizowane cookie-based przez endpoint `POST /api/account/login`.

## Najważniejsze endpointy

- `POST /api/account/login` - logowanie
- `POST /api/account/register` - rejestracja użytkownika, tylko `Admin`
- `GET /api/patients/search?query=...` - wyszukiwanie pacjenta po nazwisku/PESEL
- `GET /api/patients/{id}/visits` - lista wizyt pacjenta
- `POST /api/patients/{patientId}/records/upload` - upload skanu dokumentu
- `GET /api/visits/today` - dzisiejsze wizyty
- `GET /api/visits/active` - endpoint do NBomber, zwraca aktywne wizyty z JOIN-ami
- `GET /api/visits/{id}/card.pdf` - karta wizyty PDF
- `GET /api/visits/{id}/prescription.pdf` - recepta PDF
- `GET /api/reports/costs?patientId=&doctorId=&month=&year=&format=pdf` - raport kosztów

## Indeksy

W `ApplicationDbContext` dodano indeksy:

- `IX_Patients_Pesel` - wyszukiwanie pacjentów po PESEL.
- `IX_Patients_LastName_FirstName` - wyszukiwanie po nazwisku i imieniu.
- `IX_Visits_Doctor_StartAt` - lista wizyt lekarza w danym dniu.
- `IX_Visits_Status_StartAt` - aktywne/zaplanowane wizyty.

Do sprawozdania użyj zapytań:

```sql
SELECT * FROM Patients WHERE Pesel = '90010112345';
SELECT * FROM Visits WHERE AssignedDoctorId = '<doctor-id>' AND StartAt >= '2026-06-10' AND StartAt < '2026-06-11';
```

W folderze `docs/` są szkielety PDF do uzupełnienia screenami z Twojego lokalnego SQL Servera.

## SQL Profiler / EF Core logging

Endpoint do pokazania w profilerze: `GET /api/visits/today` albo `GET /api/visits/active`.
W `appsettings.Development.json` włączone są logi EF Core dla zapytań SQL.

## BackgroundService

Klasa `UpcomingVisitsReportBackgroundService.cs` cyklicznie generuje PDF z wizytami na kolejny dzień i wysyła go mailem przez SMTP. Domyślnie usługa jest wyłączona w konfiguracji:

```json
"BackgroundReports": { "Enabled": false }
```

Włącz ją po ustawieniu SMTP.

## CI/CD

Workflow `.github/workflows/dotnet-ci.yml` wykonuje:

- `dotnet restore`
- `dotnet build --no-restore`
- `dotnet test --no-build`
- opcjonalnie buduje obraz Docker, gdy workflow zostanie odpalony ręcznie z parametrem `buildDocker=true`.

## NBomber

Test znajduje się w `PerformanceTests/VisitsLoadTest.cs` i odpytuje `GET /api/visits/active`.
Uruchomienie:

```bash
cd PerformanceTests
dotnet run -- --baseUrl http://localhost:5000
```

Scenariusz generuje około 100 żądań przez wstrzyknięcie 50 użytkowników/s przez 2 sekundy.

## Uwagi RODO

Projekt stosuje soft delete pacjentów (`IsDeleted`) i unika twardego kasowania kartoteki. W realnym wdrożeniu należy dodać audyt dostępu do dokumentacji medycznej i ograniczyć logowanie danych wrażliwych w środowisku produkcyjnym.
