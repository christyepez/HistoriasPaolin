# Levantar ambiente desde cero

## Prompt 1 — Diagnóstico y preparación

```text
Actúa como ingeniero DevOps del repositorio christyepez/HistoriasPaolin.

Objetivo: dejar preparado un ambiente local reproducible en Windows 11 con Docker Desktop y WSL2.

1. Inspecciona el repositorio y lee AGENTS.md.
2. Verifica: Git, GitHub CLI, Node.js LTS, npm, .NET 8 SDK, Docker, Docker Compose, FFmpeg, Codex CLI y Higgsfield CLI.
3. No instales silenciosamente. Genera `scripts/check-prerequisites.ps1` y ejecútalo.
4. Para cada dependencia ausente, muestra el comando winget o npm correspondiente.
5. Verifica acceso a Docker y puertos 5080, 5081, 5432 y 5050.
6. Crea `.env.example`, pero nunca `.env` con secretos reales.
7. Crea `docs/environment-report.md` con versiones, faltantes y acciones.
8. No consumas créditos de IA y no publiques en YouTube.
9. Termina ejecutando nuevamente el diagnóstico.
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
- npm install -g @openai/codex
- npm install -g @higgsfield/cli

Después:
1. Reabre o actualiza PATH de la sesión.
2. Ejecuta `scripts/check-prerequisites.ps1`.
3. No inicies sesión con credenciales por mí.
4. Documenta los pasos manuales pendientes.
```

## Prompt 3 — Conectar Higgsfield

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

## Prompt 4 — Configurar YouTube OAuth

```text
Prepara la integración YouTube Data API v3.

1. Crea `secrets/youtube/.gitkeep` y confirma que `secrets/` está ignorado.
2. Crea `docs/integrations/youtube-oauth.md` con pasos para Google Cloud, OAuth Desktop y descarga de `client_secret.json`.
3. Implementa un comando local de autenticación inicial que guarde el token fuera de Git.
4. Valida scopes mínimos necesarios para subir, consultar y programar videos.
5. La configuración predeterminada debe ser `private`, `MadeForKids=true` y `AutoPublishEnabled=false`.
6. No realices una subida real.
```

## Prompt 5 — Levantar Docker

```text
Levanta el ambiente local completo.

1. Implementa o valida Dockerfiles y `docker-compose.yml`.
2. Servicios mínimos: api, worker, postgres y pgadmin.
3. Usa healthchecks y dependencias por condición saludable.
4. Monta `production/`, `prompts/` y `secrets/` sin copiar secretos a imágenes.
5. Ejecuta `docker compose config`, `docker compose build` y `docker compose up -d`.
6. Ejecuta migraciones.
7. Comprueba `/health`, Swagger y conexión a PostgreSQL.
8. Guarda evidencias en `docs/environment-validation.md`.
```
