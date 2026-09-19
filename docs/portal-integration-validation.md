# Validacion de integracion con PortalCorporativo

Fecha local: 2026-08-13.

## Estado

- Se reutilizo el mismo servidor SQL Server donde reside el ambiente local del Portal.
- HistoriasPaolin creo solamente `HistoriasPaolinDb`.
- No se crearon tablas de HistoriasPaolin en `PortalSecurity`, `PortalConfiguration`, `PortalMenu`, `PortalAudit`, `PortalNotification` ni `PortalIntegration`.
- HistoriasPaolin conserva integracion por adaptadores HTTP hacia el Gateway/API del Portal.
- PortalCorporativo incorpora ruta YARP local controlada hacia HistoriasPaolin.Api.
- PortalCorporativo local completo: servicios principales `healthy`, incluido Gateway en `http://localhost:8082`.
- Smoke propio de Portal (`scripts/smoke/sprint1-smoke.ps1`) ejecutado reutilizando el stack existente: `Health=Healthy`, `NotificationStatus=Sent`.

## Gateway YARP

- RouteId health: `historiaspaolin-health`.
- RouteId protegida: `historiaspaolin`.
- ClusterId: `historiaspaolin`.
- Destination: `http://historiaspaolin-api:8080/`.
- Docker network compartida: `portal-local-network`.
- Health via Gateway: `GET http://localhost:8082/api/historiaspaolin/health` devuelve `200 Healthy`.
- Endpoint protegido sin token: `GET http://localhost:8082/api/historiaspaolin/episodes` devuelve `401`.
- Endpoint protegido con token local valido: `GET http://localhost:8082/api/historiaspaolin/episodes` devuelve `200`.
- Correlation ID: validado via `X-Correlation-Id`.

## Pendientes

- Registro completo de modulo, menu y configuracion mediante APIs del Portal con token autorizado.
- Validacion ampliada de Audit API para eventos propios de HistoriasPaolin.
- Validacion ampliada de Notification API para plantillas propias de HistoriasPaolin.
- Validacion de menu filtrado por permisos cuando exista shell/menu productivo del modulo.

## Resultado disponible

- SQL Server compartido: validado.
- Base independiente: validada.
- API HistoriasPaolin: healthy.
- Worker HistoriasPaolin: healthy.
- Readiness SQL: healthy.
- Gateway Portal: healthy.
- Smoke Portal base: validado.
- Smoke Gateway -> HistoriasPaolin.Api: validado con `scripts/smoke/portal-integrated-smoke.ps1`, `PASS=7 FAIL=0 SKIP=0`.
