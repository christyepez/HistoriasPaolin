# Implementation Status

Updated: 2026-08-13

## Current State

| Field | Value |
|---|---|
| Current Sprint | Sprint 1 - Canales y estrategia editorial |
| Last Completed Task | PROMPT 06 - roadmap consolidation |
| Current Task | Ready for first functional task |
| Next Task | HP-101 - Channel Management |
| Foundation Status | DONE; do not re-run as pending |
| Build Status | Last validated OK, 0 warnings, 0 errors |
| Test Status | Last validated OK, 8/8 |
| Docker Status | API healthy, Worker healthy, migrations exited 0 |
| Portal Integration | Gateway integration completed local; integrated smoke PASS 7/0/0 |
| SQL Status | SQL operational; `HistoriasPaolinDb` ONLINE; migration applied |
| Open PRs | HistoriasPaolin #1 draft; PortalCorporativo #32 draft |

## Architectural Decisions

- Portal Gateway = EXTEND locally through PortalCorporativo PR #32.
- Security = REUSE/EXTEND; JWT and `permission` claims validated by Gateway and API.
- Correlation = REUSE; `X-Correlation-Id` validated through Gateway.
- Audit = ADAPT future adapter; no direct Portal DB access.
- Notification = ADAPT future adapter; no local notification engine.
- Menu = EXTEND pending module registration.
- Configuration = EXTEND pending module settings.
- SQL source of truth: Historias host `localhost,14333`; container `host.docker.internal,14333`.
- Portal SQL source of truth: host `localhost,21433`; container `sqlserver,1433`.

## Open Blockers

- Portal PR #32 must be reviewed/merged for a fresh Gateway route.
- Full module onboarding in Portal Menu/Configuration/Security remains pending.
- Productive IdP/OIDC remains pending; local smoke uses dev JWT only.
- Angular Shell is not required for HP-101.
