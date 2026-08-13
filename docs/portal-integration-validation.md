# Validacion de integracion con PortalCorporativo

Fecha local: 2026-08-13.

## Estado

- Se reutilizo el mismo servidor SQL Server donde reside el ambiente local del Portal.
- HistoriasPaolin creo solamente `HistoriasPaolinDb`.
- No se crearon tablas de HistoriasPaolin en `PortalSecurity`, `PortalConfiguration`, `PortalMenu`, `PortalAudit`, `PortalNotification` ni `PortalIntegration`.
- HistoriasPaolin conserva integracion por adaptadores HTTP hacia el Gateway/API del Portal.
- No se modifico codigo de PortalCorporativo.
- PortalCorporativo local completo: servicios principales `healthy`, incluido Gateway en `http://localhost:8082`.
- Smoke propio de Portal (`scripts/smoke/sprint1-smoke.ps1`) ejecutado reutilizando el stack existente: `Health=Healthy`, endpoints protegidos con `401` esperado y notificacion protegida con `AuthorizationExpected`.

## Pendiente de smoke Portal completo

El smoke Gateway -> HistoriasPaolin.Api requiere que PortalCorporativo incorpore una ruta YARP hacia `historiaspaolin-api` o que se ejecute una configuracion de integracion que conecte ambos Compose a la misma red con nombres DNS acordados.

El preflight del Gateway devolvio `404` para `GET http://localhost:8082/api/historiaspaolin/health/ready`, porque `Portal.ApiGateway/appsettings.json` no contiene una ruta `historiaspaolin` en `ReverseProxy:Routes`. El script `scripts/smoke/portal-integrated-smoke.ps1` tampoco existe aun en HistoriasPaolin.

Pendientes:

- Gateway enruta `/api/historiaspaolin/**`.
- Registro completo de modulo, menu y configuracion mediante APIs del Portal con token autorizado.
- Validacion de Audit API.
- Validacion de Notification API con proveedor local.
- Validacion de menu filtrado por permisos.

## Resultado disponible

- SQL Server compartido: validado.
- Base independiente: validada.
- API HistoriasPaolin: healthy.
- Worker HistoriasPaolin: healthy.
- Readiness SQL: healthy.
- Gateway Portal: healthy.
- Smoke Portal base: validado.
- Smoke Gateway -> HistoriasPaolin.Api: bloqueado por ruta YARP faltante.
