# Integracion Portal Corporativo

## Decision

HistoriasPaolin es modulo consumidor del PortalCorporativo. Reutiliza capacidades transversales y crea solamente dominio audiovisual propio.

## Matriz REUSE / EXTEND / ADAPT / OWN

| Capacidad | Decision | Implementacion |
|---|---|---|
| API Gateway YARP | REUSE | Consumir `api-gateway` del Portal. Ruta futura: `/api/historiaspaolin/**`. |
| Security API | EXTEND | Registrar recursos, acciones, permisos y roles `HistoriasPaolin.*`. |
| Usuarios, roles, permisos | REUSE/EXTEND | No duplicar identidad. HistoriasPaolin valida claim `permission`. |
| Menu API | EXTEND | Registrar modulo `HistoriasPaolin` y menu "Historias de Paolin". |
| Configuration API | EXTEND | Registrar claves `historiaspaolin.*` con scope de modulo. |
| Audit API | ADAPT | `PortalAuditClient` envia eventos allowlisted por Gateway. |
| Notification API | ADAPT | `PortalNotificationClient` solicita envios idempotentes. |
| SQL Outbox/Inbox | ADAPT/EXTEND | Tabla `OutboxMessages` propia en `HistoriasPaolinDb`; no escribe en bases Portal. |
| Correlation ID | REUSE | Header `X-Correlation-ID` en entrada y adaptadores HTTP. |
| Healthchecks/logging/Seq | REUSE/ADAPT | Endpoints `/health*`; Seq se consume desde la red Portal cuando este disponible. |
| Dominio episodios, escenas, prompts, FFmpeg, Higgsfield, YouTube | OWN | Propio de HistoriasPaolin. |

## Registro del modulo

- Codigo: `HistoriasPaolin`
- Nombre: `Historias de Paolin`
- Gateway interno esperado: `http://api-gateway:8080`
- API propia: `historiaspaolin-api:8080`
- Base propia: `HistoriasPaolinDb`

## Roles

- `HistoriasPaolin.Admin`
- `HistoriasPaolin.Producer`
- `HistoriasPaolin.Reviewer`
- `HistoriasPaolin.Publisher`
- `HistoriasPaolin.Viewer`

`publish` queda reservado para Admin/Publisher. Viewer, Reviewer y Producer no deben recibir `historiaspaolin.publications.publish`.

## Recursos y acciones

Recursos: `historiaspaolin.dashboard`, `historiaspaolin.episodes`, `historiaspaolin.scenes`, `historiaspaolin.characters`, `historiaspaolin.prompts`, `historiaspaolin.media`, `historiaspaolin.quality`, `historiaspaolin.publications`, `historiaspaolin.settings`, `historiaspaolin.costs`, `historiaspaolin.operations`.

Acciones: `view`, `create`, `update`, `delete`, `generate`, `approve`, `reject`, `regenerate`, `upload`, `schedule`, `publish`, `cancel`, `retry`, `configure`, `view_costs`, `view_logs`.

## Docker Compose integrado

HistoriasPaolin define `portal-local-network`. PortalCorporativo debe unirse a esa red o exponer Gateway en una red compartida equivalente. El Compose de HistoriasPaolin no crea SQL Server y usa `host.docker.internal`.

Servicios propios:

- `historiaspaolin-migrations`
- `historiaspaolin-api`
- `historiaspaolin-worker`

## Smoke tests esperados

1. `GET /health/live` de HistoriasPaolin responde sin SQL Server.
2. `GET /health/ready` falla si SQL Server no esta disponible.
3. Gateway enruta a HistoriasPaolin cuando Portal registre la ruta.
4. Token con claim `permission=historiaspaolin.episodes.create` puede crear episodio.
5. Token sin permiso recibe 403.
6. Registro de modulo llama Security, Menu y Configuration por Gateway.
7. Audit y Notification usan adaptadores HTTP y `X-Correlation-ID`.
