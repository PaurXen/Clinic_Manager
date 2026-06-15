# NBomber - raport testu wydajności

Endpoint testowany: `GET /api/visits/active`.

Scenariusz znajduje się w `PerformanceTests/VisitsLoadTest.cs`.

Konfiguracja testu:

- 50 użytkowników/s,
- czas: 2 sekundy,
- około 100 żądań,
- raport generowany do `PerformanceTests/reports/nbomber-report.html` i `.md`.

Po uruchomieniu testu wstaw do PDF ekran z wynikami: czasy odpowiedzi, throughput i błędy.
