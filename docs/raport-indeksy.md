# Raport - indeksy nieklastrowane

## Zapytanie 1: wyszukiwanie pacjenta po PESEL

```sql
SELECT * FROM Patients WHERE Pesel = '90010112345';
```

Indeks: `IX_Patients_Pesel`.

Przed indeksem spodziewany plan: skan tabeli/indeksu. Po indeksie: Index Seek po PESEL.

Miejsce na screenshot planu przed indeksem:

[WSTAW SCREENSHOT]

Miejsce na screenshot planu po indeksie:

[WSTAW SCREENSHOT]

## Zapytanie 2: wizyty lekarza w konkretnym dniu

```sql
SELECT *
FROM Visits
WHERE AssignedDoctorId = '<doctor-id>'
  AND StartAt >= '2026-06-10'
  AND StartAt < '2026-06-11';
```

Indeks: `IX_Visits_Doctor_StartAt`.

Przed indeksem spodziewany plan: scan + filtr po dacie i lekarzu. Po indeksie: Index Seek na złożonym indeksie.
