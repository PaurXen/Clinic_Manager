# Raport - nadchodzące wizyty

Raport jest generowany przez `UpcomingVisitsReportBackgroundService.cs`.

Usługa:

- pobiera wizyty zaplanowane na kolejny dzień,
- generuje PDF przez `ReportService` i `PdfReportService`,
- zapisuje plik `upcoming_visits.pdf`,
- wysyła go jako załącznik SMTP do administratora.

W trybie testowym można ustawić `BackgroundReports:IntervalMinutes` na 1 lub 2.
