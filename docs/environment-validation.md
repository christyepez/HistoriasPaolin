# Validacion local del ambiente

Fecha local: 2026-08-13.

## SQL Server

- Servidor reutilizado: instancia SQL Server existente publicada en el host.
- Puerto host validado: `14333`.
- Motor: SQL Server 2022 Developer Edition sobre Linux.
- Version observada: `16.0.4255.1`.
- Base independiente creada: `HistoriasPaolinDb`.
- Estado de base: `ONLINE`.
- Compatibility level: `160`.
- Login dedicado creado: `historiaspaolin_app`.
- Permisos locales asignados: `db_datareader`, `db_datawriter`, `db_ddladmin`.
- No se concedio `sysadmin`.
- No se modificaron bases del Portal.

## Migraciones

- Migracion aplicada: `20260803220000_InitialSqlServerSchema`.
- `dbo.__EFMigrationsHistory`: creada y con registro de la migracion.
- Tablas creadas: `Episodes`, `EpisodeScenes`, `OutboxMessages`.
- FK validada: `FK_EpisodeScenes_Episodes_EpisodeId`.
- Indices validados: `IX_Episodes_Status`, `UX_EpisodeScenes_EpisodeId_SortOrder`, `UX_OutboxMessages_TenantId_IdempotencyKey`, `IX_OutboxMessages_Status_OccurredAtUtc`.
- Auditoria validada por esquema: `CreatedAtUtc`, `UpdatedAtUtc`, `CreatedBy`, `UpdatedBy`.
- Concurrencia validada por esquema: columnas `RowVersion`.
- Decimal validado: `EstimatedCostUsd decimal(18,4)`.

## Docker

- Contenedor SQL Server reutilizado: existente y `healthy`.
- Docker desde Windows hacia SQL: OK con `localhost:14333`.
- Docker desde contenedor temporal hacia SQL: OK con `host.docker.internal:14333`.
- `docker compose config --quiet`: OK. No registrar la salida completa de `docker compose config` porque expande variables locales.
- `docker compose build`: OK.
- `docker compose up -d`: OK.
- `historiaspaolin-migrations`: finalizo correctamente.
- `historiaspaolin-api`: `healthy`, puerto `5080`.
- `historiaspaolin-worker`: `healthy`.

## Healthchecks

- `GET http://localhost:5080/health/ready`: `200 Healthy`.
- `GET http://localhost:5080/health`: `200 Healthy`.
- `GET http://localhost:5080/health/live`: `200 Healthy`.
- Swagger: `http://localhost:5080/swagger/index.html` responde `200`.

## Build y pruebas

- `dotnet restore HistoriasPaolin.sln`: OK.
- `dotnet build HistoriasPaolin.sln --no-restore`: OK, 0 warnings, 0 errores en ejecucion secuencial final.
- `dotnet test HistoriasPaolin.sln --no-build`: OK, 8 pruebas aprobadas.
- `dotnet test HistoriasPaolin.sln --no-build --filter RequiresSqlServer`: OK sin ejecucion de casos porque no hay pruebas etiquetadas con `RequiresSqlServer`.
- Prueba SQL real desde contenedor temporal: OK con `host.docker.internal:14333`, base `HistoriasPaolinDb` y usuario `historiaspaolin_app`.

## Seguridad

- `.env` de HistoriasPaolin y PortalCorporativo estan ignorados por Git.
- No se versionaron secretos reales.
- `git grep` solo encontro placeholders o nombres de variables para `SQLSERVER_PASSWORD` y `JWT_SECRET`.
- La clave local de `historiaspaolin_app` se genero sin `$` para evitar interpolacion accidental en Docker Compose.
- No se uso Higgsfield.
- No se genero video.
- No se conecto YouTube.
