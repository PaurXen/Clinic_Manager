# Raport - SQL Profiler / EF Core Logging

Endpoint: `GET /api/visits/active`.

Endpoint pobiera aktywne wizyty i łączy tabele: `Visits`, `Patients`, `AspNetUsers`, `ProceduresPerformed`, `PrescribedMedications`, `Medications`.

## Kroki

1. Uruchom aplikację lokalnie.
2. Otwórz SQL Server Profiler albo obserwuj logi EF Core w konsoli.
3. Wywołaj `GET /api/visits/active` ze Swaggera.
4. Wstaw screenshot zapytania SQL i krótko opisz JOIN-y oraz filtr statusu.

[WSTAW SCREENSHOT PROFILERA]
