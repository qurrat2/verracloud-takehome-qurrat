# Portfolio Holdings Dashboard

A small full-stack app that tracks stock holdings and shows live unrealized P&L. Prices are simulated and refresh automatically every 10 seconds.

- **Backend:** ASP.NET Core 10 Web API, EF Core 10, SQLite
- **Frontend:** React + Vite, Redux Toolkit (RTK Query)

## Prerequisites

- .NET 10 SDK
- Node.js 20+

## Run

**Backend** (http://localhost:5282, Swagger at `/swagger`):

```bash
cd Portfolio.Api
dotnet run
```

The database is created, migrated, and seeded automatically on startup.

**Frontend** (http://localhost:5173):

```bash
cd frontend
npm install
npm run dev
```

**Tests:**

```bash
dotnet test
```

## Configuration

The frontend API base URL defaults to `http://localhost:5282/api/`. To override it, copy `frontend/.env.example` to `frontend/.env` and set `VITE_API_URL`.

## API

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/api/holdings` | Holdings with current price, market value, and P&L |
| POST | `/api/holdings` | Add a holding |
| DELETE | `/api/holdings/{id}` | Remove a holding |
| GET | `/api/prices` | All tickers with current price |
| GET | `/api/prices/history` | Recent price series for held tickers |
| POST | `/api/prices/refresh` | Manually move prices +/-2% |
| POST | `/api/seed/reset` | Reset all data to seed defaults (dev only) |

## Architecture

The backend is layered: **Controller -> Service -> Repository**.

- Controllers handle HTTP only; EF entities never cross the boundary (DTOs do).
- Services hold the business rules and the P&L calculation `(currentPrice - purchasePrice) * quantity`.
- Repositories isolate EF Core / SQLite access.
- A `BackgroundService` refreshes prices every 10s; a global exception handler maps errors to RFC 7807 `ProblemDetails`.

The frontend uses RTK Query to fetch and cache data, polling holdings/prices every 5s so the UI tracks the background price changes.

## Design decisions

**Why the backend is structured this way.** The layered split keeps each concern testable and swappable: services are unit-tested with mocked repositories, and the repository boundary means the database could move off SQLite without touching business logic. DTOs at the HTTP boundary keep the persistence model private.

**Schema: one `Tickers` table, not a separate `Prices` table.** Price is 1:1 with a ticker and carries no history of its own, so it lives on `Tickers`. `GET /api/prices` still returns the spec-shaped `{ ticker, currentPrice, lastUpdatedAt }`. A separate `Price` history table exists only to back the trend chart.

**How to scale to 10,000 concurrent users.** This build is a demo: SQLite is a single-writer file database and the client polls every 5s. To scale I would move to Postgres/SQL Server with connection pooling, replace polling with server push (SignalR/WebSockets) backed by a Redis cache for prices, run the price refresh as a separate worker, put the stateless API behind a load balancer, and serve the SPA from a CDN.

**What I would improve with another 2 hours.** Add a `Users` table with authentication/authorization so each user logs in and sees only their own holdings. Support partial sells, since deleting a holding currently removes the whole position when a user may want to reduce the quantity and keep the rest. Persist a few months of per-user P&L history to chart performance over time. Replacing the 5s polling with SignalR push would be a follow-up after that.

## Edge cases covered

- Negative or zero quantity is rejected with a clear message (validated server-side, unit tested).
- Negative purchase price is rejected (validated server-side, unit tested).
- Unknown ticker on add returns a 400 naming the ticker code.
- Fractional share quantities are supported via `decimal` (unit tested).
- A ticker with no price yet (`currentPrice = 0`) computes safely server-side (unit tested) and shows "Not priced yet" / "N/A" in the table instead of a misleading `$0.00`.
- Losses show negative P&L in red; gains in green.
- Multiple holdings of the same ticker are allowed; adding an identical holding within 2 minutes prompts a confirmation.
- Add and delete both ask for confirmation.
- Every data view has explicit loading and error states; a failed background poll keeps showing the last good values rather than blanking the UI.
- The holdings table filters by ticker and paginates at 5 rows per page.
- SQLite runs in WAL mode so polling reads do not block the background refresh writes, and the refresh logs and continues on failure rather than killing the loop.

## Notes (dev only)

- CORS allows any `localhost` origin so the frontend works on any Vite port; production should pin the real origin.
- `POST /api/seed/reset` is an unauthenticated convenience endpoint and would be removed or secured in production.
- SQLite runs in WAL mode so polling reads do not block the background refresh writes.
- Optimistic concurrency is not implemented: SQLite has no server-side rowversion, so it would be a no-op here.

See `docs/ai-usage-log.md` for how AI tools were used on this project.

Thanks for reviewing.
Regards,
Qurrat
