  ## Project
  ASP.NET Core 10 Web API plus a React/Redux frontend. A portfolio dashboard that tracks bond and equity holdings with simulated market prices and unrealized P&L.

  ## Stack
  - Backend: .NET 10, ASP.NET Core Web API with controllers (not minimal APIs), EF Core with SQLite, Swashbuckle for Swagger in development
  - Frontend (added in a later layer): Vite plus React plus Redux Toolkit
  - Tests (added in a later layer): xUnit

  ## Principles
  - Layered backend: Controller → Service → Repository. Keep business logic out of controllers.
  - DTOs at the HTTP boundary. EF entities never cross into HTTP responses.
  - The 10-second price refresh is a `BackgroundService` using `PeriodicTimer` and `IServiceScopeFactory`. Never inject `DbContext` directly into a hosted service.
  - Validation produces meaningful, specific error messages.
  - P&L is calculated server-side, not in the client.

  ## Avoid
  - Minimal APIs (the spec wants controllers)
  - AutoMapper or generic `Repository<T>` (project is small; explicit code stays clearer)
  - `WeatherForecast` template debris (it's been deleted)
  - Comments that restate code
  - `try/catch` without a meaningful handler

  ## Working notes
  - Spec: `docs-local/VerraCloud Project TEST.pdf` (proprietary; folder is gitignored, local-only). The canonical source of truth for endpoint contracts, validation rules, and the database schema. Attach to chat context when working on contract-bound decisions
  - Decisions and overrides are logged in `docs/ai-decisions.md` as the project progresses
  - This file grows with the project; rules for layers not yet built will be appended when those layers land