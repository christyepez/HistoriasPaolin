# First Task

TaskId: HP-101
Sprint: 1 - Canales y estrategia editorial
Title: Channel Management
Status: DONE

Objective:
Create the first functional vertical slice for managing independent content channels with editorial configuration, locale, budget and enabled provider metadata. Persist only in `HistoriasPaolinDb`.

DependsOn:
HP-006, HP-0A02

PrimaryAgent:
Backend Architect

ReviewAgent:
Solution Architect

Skill:
channel-strategy

PortalCapability:
Configuration EXTEND; Menu EXTEND later; Security REUSE.

FilesAllowed:
`src/HistoriasPaolin.Api/Controllers`, `src/HistoriasPaolin.Contracts`, `src/HistoriasPaolin.Application`, `src/HistoriasPaolin.Domain`, `src/HistoriasPaolin.Infrastructure`, `tests`, `database`, `docs`, `codex`.

FilesForbidden:
`.env`, `secrets`, `tokens`, PortalCorporativo source, CodexCommonAgents source, generated media, `bin`, `obj`.

DatabaseImpact:
Add Channel/ChannelBrand tables and EF migration in `HistoriasPaolinDb`; no cross-database access; no Portal tables.

SecurityImpact:
Require authenticated API access and permission `historiaspaolin.channels.manage`; no IdP production dependency.

AcceptanceCriteria:
- Channel aggregate supports name, niche, audience, language, country, frequency, budget, status and enabled providers.
- API supports create/list/get/update/activate/deactivate using vertical architecture.
- Persistence uses SQL Server EF Core in `HistoriasPaolinDb`.
- Validation prevents invalid budgets, locales and empty strategy fields.
- No paid provider calls.
- No Angular Shell dependency.

ValidationCommands:
`dotnet build`
`dotnet test`
`git diff --check`
`git status --short`

Result:
HP-101 implemented. HP-102 implemented. HP-103 implemented. Next READY task is HP-104.
