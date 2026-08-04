# Validacion de integracion con PortalCorporativo

Fecha local: 2026-08-03.

## Estado

- Se reutilizo el mismo servidor SQL Server donde reside el ambiente local del Portal.
- HistoriasPaolin creo solamente `HistoriasPaolinDb`.
- No se crearon tablas de HistoriasPaolin en `PortalSecurity`, `PortalConfiguration`, `PortalMenu`, `PortalAudit`, `PortalNotification` ni `PortalIntegration`.
- HistoriasPaolin conserva integracion por adaptadores HTTP hacia el Gateway/API del Portal.
- No se modifico codigo de PortalCorporativo.

## Pendiente de smoke Portal completo

El smoke Gateway -> HistoriasPaolin.Api requiere que PortalCorporativo incorpore una ruta YARP hacia `historiaspaolin-api` o que se ejecute una configuracion de integracion que conecte ambos Compose a la misma red con nombres DNS acordados.

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
