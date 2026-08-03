# Levantar ambiente desde cero

## Decisión de infraestructura local

- Todo componente desplegable se ejecuta en Docker Compose local.
- `api` y `worker` se ejecutan en contenedores.
- Se utilizará la instancia SQL Server que ya existe en el equipo o red local.
- No se levantará PostgreSQL ni SQL Server dentro de Docker Compose.
- La base nueva se llamará `HistoriasPaolinDb`.
- La instancia, puerto, autenticación y credenciales se configuran fuera de Git.

Variables esperadas en `.env`:

```env
SQLSERVER_HOST=host.docker.internal
SQLSERVER_PORT=1433
SQLSERVER_INSTANCE=
SQLSERVER_DATABASE=HistoriasPaolinDb
SQLSERVER_USER=
SQLSERVER_PASSWORD=
SQLSERVER_ENCRYPT=true
SQLSERVER_TRUST_SERVER_CERTIFICATE=true
```

Cuando la instancia sea nombrada, `SQLSERVER_HOST` puede conservar el host y `SQLSERVER_INSTANCE` debe contener el nombre de instancia. Si la instancia utiliza puerto fijo, se recomienda conectarse por host y puerto desde Docker.

## Prompt 1 — Diagnóstico y preparación

```text
Actúa como ingeniero DevOps del repositorio christyepez/HistoriasPaolin.

Objetivo: dejar preparado un ambiente local reproducible en Windows 11 con Docker Desktop y WSL2.

1. Inspecciona el repositorio y lee AGENTS.md.
2. Verifica: Git, GitHub CLI, Node.js LTS, npm, .NET 8 SDK, Docker, Docker Compose, FFmpeg, Codex CLI, Higgsfield CLI, sqlcmd y conectividad con SQL Server.
3. No instales silenciosamente. Genera `scripts/check-prerequisites.ps1` y ejecútalo.
4. Para cada dependencia ausente, muestra el comando winget o npm correspondiente.
5. Verifica acceso a Docker y puertos 5080 y 5081.
6. Solicita mediante variables locales el host, puerto o instancia y el tipo de autenticación de SQL Server. No escribas credenciales en documentación ni Git.
7. Desde el host y desde un contenedor temporal valida la conexión a SQL Server.
8. Crea `.env.example`, pero nunca `.env` con secretos reales.
9. Crea `docs/environment-report.md` con versiones, faltantes y acciones.
10. No consumas créditos de IA y no publiques en YouTube.
11. Termina ejecutando nuevamente el diagnóstico.
```

## Prompt 2 — Instalación de herramientas

```text
Lee `docs/environment-report.md` y corrige únicamente dependencias faltantes.

Comandos esperados cuando apliquen:
- winget install Git.Git
- winget install GitHub.cli
- winget install OpenJS.NodeJS.LTS
- winget install Microsoft.DotNet.SDK.8
- winget install Docker.DockerDesktop
- winget install Gyan.FFmpeg
- instalar Microsoft sqlcmd u ODBC Driver 18 para SQL Server
- npm install -g @openai/codex
- npm install -g @higgsfield/cli

Después:
1. Reabre o actualiza PATH de la sesión.
2. Ejecuta `scripts/check-prerequisites.ps1`.
3. No inicies sesión con credenciales por mí.
4. Documenta los pasos manuales pendientes.
```

## Prompt 3 — Preparar SQL Server existente

```text
Configura la aplicación para utilizar la instancia SQL Server existente.

1. Lee AGENTS.md y no agregues un contenedor de base de datos.
2. Detecta las variables `SQLSERVER_HOST`, `SQLSERVER_PORT`, `SQLSERVER_INSTANCE`, `SQLSERVER_USER` y `SQLSERVER_PASSWORD` desde `.env` local.
3. Valida conectividad desde Windows con sqlcmd.
4. Valida conectividad desde un contenedor temporal en la misma red de Docker Compose.
5. Genera `scripts/sql/create-database.sql` de manera idempotente para crear `[HistoriasPaolinDb]` si no existe.
6. Genera `scripts/sql/create-database.ps1` para ejecutar el SQL sin exponer la contraseña en logs.
7. Usa `Microsoft.EntityFrameworkCore.SqlServer`.
8. Implementa `DesignTimeDbContextFactory` para migraciones.
9. Crea la migración inicial.
10. Ejecuta la creación de base y luego `dotnet ef database update` desde un servicio de migración o comando Docker Compose.
11. Verifica tablas, historial de migraciones y healthcheck de base.
12. Si la cuenta configurada no posee permiso `CREATE DATABASE`, detente y documenta el permiso requerido; no intentes elevar privilegios.
13. Nunca almacenes credenciales reales en Git.
```

SQL idempotente esperado:

```sql
IF DB_ID(N'HistoriasPaolinDb') IS NULL
BEGIN
    CREATE DATABASE [HistoriasPaolinDb];
END;
GO
```

## Prompt 4 — Conectar Higgsfield

```text
Configura Higgsfield sin exponer credenciales.

1. Verifica `higgsfield --version` y `higgsfield auth status`.
2. Si no existe sesión, indícame ejecutar `higgsfield auth login`.
3. Consulta `higgsfield model list --json`.
4. Consulta el esquema real de los modelos de imagen y Seedance 2.0.
5. Guarda únicamente nombres, capacidades y parámetros públicos en `docs/integrations/higgsfield-models.md`.
6. Configura MCP en `~/.codex/config.toml` con `https://mcp.higgsfield.ai/mcp`, conservando cualquier configuración existente.
7. Verifica con `codex mcp list`.
8. No generes contenido ni consumas créditos.
```

## Prompt 5 — Configurar YouTube OAuth

```text
Prepara la integración YouTube Data API v3 para una fase posterior al piloto local.

1. Crea `secrets/youtube/.gitkeep` y confirma que `secrets/` está ignorado.
2. Crea `docs/integrations/youtube-oauth.md` con pasos para Google Cloud, OAuth Desktop y descarga de `client_secret.json`.
3. Implementa un comando local de autenticación inicial que guarde el token fuera de Git.
4. Valida scopes mínimos necesarios para subir, consultar y programar videos.
5. La configuración predeterminada debe ser `private`, `MadeForKids=true`, `PublishingProvider=DryRun` y `AutoPublishEnabled=false`.
6. No realices una subida real.
```

## Prompt 6 — Levantar Docker Compose local

```text
Levanta el ambiente local completo.

1. Implementa o valida Dockerfiles y `docker-compose.yml`.
2. Servicios mínimos: api, worker y migrations.
3. No agregues PostgreSQL, pgAdmin, SQL Server ni otra base de datos al Compose.
4. Los servicios deben conectarse a la instancia SQL Server existente mediante variables de entorno.
5. Usa healthchecks y dependencias por condición saludable.
6. Monta `production/`, `prompts/`, `secrets/` y `tokens/` sin copiar secretos a imágenes.
7. Ejecuta `docker compose config` y verifica que no muestre secretos en logs o artefactos.
8. Ejecuta `docker compose build`.
9. Ejecuta primero el perfil o servicio `migrations` para crear `HistoriasPaolinDb` y aplicar migraciones.
10. Ejecuta `docker compose up -d`.
11. Comprueba `/health`, Swagger, Worker y conexión a SQL Server.
12. Guarda evidencias en `docs/environment-validation.md`.
```
