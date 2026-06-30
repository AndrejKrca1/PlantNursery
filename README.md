# PlantNursery 🌱

Aplikacija za upravljanje kućnim vrtom i biljkama. Prati zalijevanje, gnojenje,
presađivanje i zdravstveni status svake biljke uz kalendar njege.

## Struktura projekta

- **PlantNursery.Core** – logika, modeli, kolekcije, asinkronost, komunikacija s okolinom
- **PlantNursery.WinForms** – pregled kolekcije biljaka, detalji biljke
- **PlantNursery.WPF** – vizualni kalendar njege

## Pokretanje

```bash
dotnet restore
dotnet build
dotnet run --project PlantNursery.WinForms
dotnet run --project PlantNursery.WPF
```

## Pokrivenost ishoda

| Ishod | Gdje |
|-------|------|
| I1 – C# sintaksa | `Enums.cs`, `Plant.cs` (enum, formatiranje stringa, petlje, nizovi, XML komentari) |
| I2 – OOP osnove | `Plant.cs`, `PlantTypes.cs` (apstraktna klasa, nasljeđivanje, sučelje, virtualna metoda, enkapsulacija) |
| I3 – Napredni OOP i kolekcije | `PlantManager.cs` (List, Dictionary, Queue, SortedList, event, iznimka, struktura, generici) |
| I4 – Višedretvenost i async | `CareService.cs` (Thread, Task.Run, SemaphoreSlim, async/await, delegat) |
| I5 – GUI | WinForms + WPF (3 prozora, TreeView, ListView, ProgressBar, Calendar, MenuStrip...) |
| I6 – Komunikacija s okolinom | `DataService.cs`, `PlantDbContext.cs` (XML, JSON, CSV, HTML, EF Core SQLite, TCP) |

## Podaci

- `data/katalog.csv` – katalog biljaka s frekvencijama njege
- `data/settings.json` – postavke aplikacije
