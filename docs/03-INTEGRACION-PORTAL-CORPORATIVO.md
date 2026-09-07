# Integracion Portal Corporativo

Usar este documento antes de implementar Sprint 0 o cualquier capacidad transversal.

## Decision

HistoriasPaolin es modulo consumidor del PortalCorporativo. Debe conservar su dominio, base de datos y pipeline audiovisual propios, pero debe reutilizar seguridad, gateway, navegacion, configuracion, auditoria, notificaciones, observabilidad e integracion transversal del portal.

## Inspeccion obligatoria

Leer en PortalCorporativo:

- `README.md`
- `AGENTS.md`, si existe
- `docs/coordination/consumer-onboarding-guide.md`
- `codex/REUSABLE_CAPABILITIES.md`
- `docs/security/authorization-policy-matrix.md`
- `docs/local-development.md`
- Compose real y contratos Gateway, Security, Configuration, Menu, Audit, Notification y Outbox/Inbox

Leer en HistoriasPaolin:

- `AGENTS.md`
- `docs/01-LEVANTAR-AMBIENTE.md`
- `docs/02-PROMPTS-IMPLEMENTACION.md`
- `docs/SPRINTS.md`
- `docs/03-INTEGRACION-PORTAL-CORPORATIVO.md`
- todos los agentes dentro de `.codex/agents/`

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

## Arquitectura

1. Trafico externo entra por Gateway YARP de PortalCorporativo.
2. Gateway enruta a `historiaspaolin-api`.
3. Security API es fuente de verdad de usuarios, roles, recursos, permisos y autorizacion.
4. HistoriasPaolin valida autorizacion backend por recursos y acciones del portal.
5. Menu API expone modulo y opciones filtradas por permisos.
6. Configuration API administra parametros no secretos.
7. Audit API recibe eventos de auditoria.
8. Notification API procesa notificaciones con plantillas e idempotencia.
9. Outbox local en `HistoriasPaolinDb` garantiza entrega sin transacciones distribuidas.
10. `X-Correlation-ID` viaja por Gateway, API, Worker, SQL, logs y eventos.

No se permite acceso directo entre bases, claves foraneas entre bases, duplicacion definitiva de capacidades transversales ni copia de proyectos completos del portal.

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

## Menu sugerido

- Dashboard
- Episodios
- Produccion
- Control de calidad
- Publicaciones
- Costos
- Configuracion
- Operaciones

Cada item debe tener recurso, permiso, ruta, orden, icono y visibilidad filtrada en backend.

## Configuracion no secreta

Usar Configuration API para modelos predeterminados, duracion objetivo, escenas maximas, presupuesto diario, regeneraciones, horarios, idioma, pais, `madeForKids`, `AutoPublishEnabled`, `RequireHumanApproval`, umbral de calidad, retencion y limites de frecuencia.

No guardar contrasenas SQL Server, tokens YouTube, `client_secret`, tokens Higgsfield ni claves privadas en Configuration API.

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

## Sprint 0A

- HP-0A01 Inventario y matriz REUSE/EXTEND/ADAPT/OWN.
- HP-0A02 Integracion Gateway y Security.
- HP-0A03 Registro Menu y Configuration.
- HP-0A04 Adaptadores Audit, Notification y Outbox.
- HP-0A05 Compose local integrado y smoke tests.
