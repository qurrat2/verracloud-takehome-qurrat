# AI Usage Log

## Tools used
- **GitHub Copilot** — chat plus three custom agent skills (architect / reviewer / tester); primary driver for scaffolding and the data and service layers.
- **Claude (Claude Code)** — after the run of Copilot misfires below, I switched to Claude for pair programming and added a `CLAUDE.md` pointing it at the same `.github` skills, so the architect / reviewer / tester guidance carried over.

## How AI helped
- Scaffolded the EF Core entities, `PortfolioDbContext`, and the repository layer from a short prompt, which I then adjusted by hand.
- The architect skill surfaced the high-leverage modeling calls up front: `decimal(18,4)` for money and quantity, `DateTimeOffset` timestamps, and fractional-share `decimal` quantities.
- Claude restructured the project into a clean Controller to Service to Repository layout, consolidated the startup wiring, regenerated the migration, and confirmed the API builds, seeds, and serves end-to-end.

## Where AI was wrong or suboptimal (caught and dropped)
- Copilot proposed an over-engineered schema (Users, Currency, PriceHistory, Volume, Instrument); dropped it and scoped back to the spec's two tables.
- Copilot suggested `RowVersion` concurrency tokens; dropped them after they crashed seeding on SQLite, which has no server-side rowversion, and regenerated a clean migration.
- Copilot scaffolded the data layer but omitted the EF Core NuGet packages; caught by inspection before the build and added them.
- Copilot left out the `Program.cs` DI and migrate/seed wiring that was part of the same prompt; wrote it by hand.
- Copilot first configured a unique index on `Holdings.TickerId`; relaxed it to non-unique so one ticker can hold multiple positions.
- Copilot omitted the `Holdings` to `Tickers` foreign key; added it with `OnDelete(Restrict)`.
- AI left the wired `Program.cs` and connection string at the repo root, outside the project, so the app never actually ran; moved them into `Portfolio.Api` and deleted the strays.
- The generated test project referenced the API with the wrong relative path, so the tests never built; fixed the reference.

## Reflection
AI was fastest at the mechanical work: scaffolding, boilerplate, and surfacing standard modeling conventions. It was least reliable on cross-file wiring and on assumptions that do not hold for the chosen stack (SQLite rowversion, project-relative paths, where generated files land). The pattern I leaned on was to treat AI output as a first draft to inspect against the spec and actually run, not as finished code.
