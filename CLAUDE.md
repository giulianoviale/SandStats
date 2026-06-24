# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build SandStats.csproj

# Run (development — uses SQLite at app.db)
dotnet run

# EF Core migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update

# Docker build & run
docker build -t sandstats .
docker run -p 8080:8080 -e DATABASE_URL=postgres://user:pass@host:port/db sandstats
```

No automated test project exists in this repo.

## Language

The codebase is written in **Spanish**: entity names, properties, page names, variables, and comments are all in Spanish. Keep this convention when adding new code.

## Architecture

ASP.NET Core 9.0 **Razor Pages** application (not MVC). All pages live under `Pages/` and follow the `PageModel` + `.cshtml` pair pattern.

**Database:** SQLite in development (`app.db`), PostgreSQL in production via the `DATABASE_URL` env var (postgres:// URI format). The provider is selected at startup in `Program.cs` by checking for `"Host="` in the connection string. All `DateTime` values are normalized to UTC by overrides in `ApplicationDbContext.SaveChanges*`.

**Auth:** ASP.NET Identity with custom `AppSignInManager` (allows login by username or email, supports `IsActive` and `MustResetPassword` flags). All pages are authorized by default; only `/Identity/**` and `/Index` are anonymous.

## Domain Model

The app tracks **beach volleyball (sand) statistics**. Core entities:

| Entity | Description |
|--------|-------------|
| `Jugador` | Player |
| `Dupla` | Player pair (unique pair, links two Jugadores) |
| `Partido` | Match (two Duplas, date, tournament, climate, set results) |
| `Set` | Individual set within a Partido |
| `EstadisticaAtaque` | Attack stats: Lado (Bueno/Medio/Atras), TipoAcciones (Atq1-9, Tl1-9, Td1-9, etc.), ResultadoAtaque |
| `EstadisticaRecepcion` | Reception stats: zone, serve type |
| `EstadisticaK2` | Block/point stats |
| `VideoLinksJugadorPartido` | Per-player, per-match video links |
| `JugadorLinks` | General video links per player |

All statistic entities support `ScopeEstadistica` (PartidoCompleto vs Cierre) to distinguish full-match stats from "closing points" stats. The `DesdePunto` and `SetNumero` fields define the closing scope window.

## Key Relationships (from OnModelCreating)

- `Dupla` → two `Jugador` (Restrict delete, unique index on pair)
- `Partido` → two `Dupla` (Restrict delete)
- `Set` → `Partido` (Cascade delete)
- `EstadisticaAtaque/Recepcion/K2` → `Partido` (Cascade) + `Jugador` (Restrict)
- `VideoLinksJugadorPartido` → both `Partido` and `Jugador` (Cascade)

## Pages Structure

- `Pages/Jugadores/` — player CRUD + video links
- `Pages/Duplas/` — pair CRUD with pagination (10/page)
- `Pages/Partidos/` — match CRUD
- `Pages/Estadisticas/` — stat entry (CargarAtaques, CargarRecepciones, CargarK2) and editing pages
- `Pages/Estadisticas/Partials/` — reusable report partial pages (attack map, reception summary, K2, etc.)
- `Pages/Reportes/` — report selector
- `Pages/Admin/Users/` — user administration
- `Areas/Identity/` — scaffolded Identity pages (login, register)

## Migrations

Migrations are in `Migrations/`. New schema changes require a new migration — avoid editing existing ones that have been applied to production. The app does **not** auto-run migrations on startup; run `dotnet ef database update` manually or via deployment pipeline.

## Convenciones de trabajo

- **Rama de trabajo:** Siempre trabajar sobre `feature/carga-en-vivo`. Nunca sobre `dev` ni `main`/`master`.
- **Verificación de cambios:** Ejecutar `dotnet build SandStats.csproj` antes de dar cualquier cambio por terminado (no existen tests automatizados).
- **Aislamiento del módulo de carga en vivo:** Es un contexto independiente. Comparte únicamente las entidades de master data (`Jugador`, `Dupla`) con el sistema existente. No acoplarlo al resto de la aplicación.
- **Convenciones del proyecto:** Código y nombres de dominio en español, patrón Razor Pages (`PageModel` + `.cshtml`), migraciones EF Core manuales (`dotnet ef migrations add` + `dotnet ef database update`).
