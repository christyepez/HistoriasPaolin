# Historias de Paolín

Plataforma para producir, validar y publicar episodios infantiles originales mediante Codex, Higgsfield AI, Seedance 2.0, FFmpeg y YouTube Data API.

## Estado

Repositorio inicializado. La documentación de ambiente, arquitectura, agentes, prompts y sprints se desarrolla en la rama `feature/bootstrap-agents-sprints`.

## Principios

- Contenido infantil original y seguro.
- Arquitectura vertical en .NET 8.
- Automatización idempotente y auditable.
- Portal-first: reutilizar PortalCorporativo para Gateway, seguridad, permisos, menu, configuracion, auditoria, notificaciones, healthchecks, logging y correlacion.
- Generación de máximo un episodio por ejecución.
- Publicación inicialmente privada y sujeta a control de calidad.
- Secretos fuera del repositorio.

## Integracion Portal Corporativo

HistoriasPaolin es un consumidor del PortalCorporativo. Mantiene como propios el dominio audiovisual, `HistoriasPaolinDb`, prompts, Higgsfield, Seedance, FFmpeg, control de calidad, YouTube y worker audiovisual.

No consulta tablas del Portal y el Portal no consulta `HistoriasPaolinDb`. La integracion se realiza mediante Gateway, APIs, contratos HTTP y eventos/outbox local.

## Local

```powershell
dotnet restore HistoriasPaolin.sln
dotnet build HistoriasPaolin.sln --no-restore
dotnet test HistoriasPaolin.sln
docker compose config
```

Copie `.env.example` a `.env` y complete valores locales antes de usar SQL Server o Docker Compose. La API expone `/health`, `/health/live`, `/health/ready` y Swagger en Development.

Validacion local registrada en `docs/environment-validation.md` y `docs/portal-integration-validation.md`.
