# Portal Gateway integration

Fecha local: 2026-08-13.

## Gateway

- Gateway owner: `PortalCorporativo`.
- Gateway route health: `/api/historiaspaolin/health/{**catch-all}`.
- Gateway route protegida: `/api/historiaspaolin/{**catch-all}`.
- Gateway cluster: `historiaspaolin`.
- Destination: `http://historiaspaolin-api:8080/`.
- Docker network compartida: `portal-local-network`.
- Docker hostname downstream: `historiaspaolin-api`.

## Seguridad

- Health via Gateway queda anonimo solo para `/api/historiaspaolin/health/**`.
- La ruta protegida conserva `AuthorizationPolicy: default` en Portal Gateway.
- HistoriasPaolin.Api valida JWT nuevamente y exige claim `permission`.
- Permiso minimo probado: `historiaspaolin.episodes.view`.
- No se implementa login propio ni emisor JWT alternativo en HistoriasPaolin.

## Correlacion

- Header esperado: `X-Correlation-Id`.
- HistoriasPaolin normaliza y devuelve `X-Correlation-ID`.
- El smoke verifica que el correlation ID enviado vuelva en la respuesta Gateway -> HistoriasPaolin.

## URLs locales

- Portal Gateway: `http://localhost:8082`.
- HistoriasPaolin directo: `http://localhost:5080`.
- Health directo: `http://localhost:5080/health`.
- Health via Gateway: `http://localhost:8082/api/historiaspaolin/health`.
- Endpoint protegido via Gateway: `http://localhost:8082/api/historiaspaolin/episodes`.

## Smoke

Comando con token ya emitido:

```powershell
.\scripts\smoke\portal-integrated-smoke.ps1 `
  -PortalGatewayUrl http://localhost:8082 `
  -HistoriasPaolinUrl http://localhost:5080 `
  -JwtToken "<token-local>"
```

Comando local de desarrollo con secreto JWT fuera de Git:

```powershell
.\scripts\smoke\portal-integrated-smoke.ps1 `
  -PortalGatewayUrl http://localhost:8082 `
  -HistoriasPaolinUrl http://localhost:5080 `
  -JwtSecret "<jwt-secret-local>"
```

El script reporta `PASS`, `FAIL` y `SKIP`. `SKIP` no se cuenta como `PASS`.

## SQL Server local

- Fuente de verdad para HistoriasPaolin desde Windows: `localhost,14333`.
- Fuente de verdad para HistoriasPaolin desde contenedor: `host.docker.internal,14333`.
- Instancia observada en `14333`: contenedor `requirements-sqlserver`, contiene `HistoriasPaolinDb`.
- Fuente de verdad para SQL de Portal desde Windows: `localhost,21433`.
- Fuente de verdad para SQL de Portal desde contenedores de Portal: `sqlserver,1433`.
- Instancia observada en `21433`: contenedor `portal-corporativo-sqlserver-1`, contiene bases del Portal.

`14333` y `21433` son instancias SQL Server distintas. No se debe asumir que ambas apuntan al mismo motor.

## Limitaciones conocidas

- La ruta YARP se activo para integracion local controlada de HistoriasPaolin.
- El IdP/OIDC productivo sigue pendiente; el smoke puede usar token local de desarrollo firmado con el secreto local de Portal.
- No se consultan bases del Portal desde HistoriasPaolin.
- No se implementan Higgsfield, Seedance, generacion multimedia ni publicacion social en esta etapa.
