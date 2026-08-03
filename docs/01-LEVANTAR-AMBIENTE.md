# Levantar ambiente desde cero

## Regla general

Todo componente desplegable debe ejecutarse exclusivamente en Docker Compose local. Primero se validará el flujo completo local: API, Worker, PostgreSQL, pgAdmin, FFmpeg, generación de recursos, ensamblaje, control de calidad y simulación de publicación. Solo después se integrará la cuenta real de YouTube mediante OAuth 2.0.

No usar Azure, AWS, Google Cloud, Kubernetes, App Service, Cloud Run, servidores externos ni servicios PaaS para desplegar la aplicación.

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

## Prompt 4 — Preparar YouTube sin publicar

```text
Prepara la integración futura con YouTube Data API v3, pero no conectes todavía la cuenta real ni subas videos.

1. Crea `secrets/youtube/.gitkeep` y confirma que `secrets/` está ignorado.
2. Crea `docs/integrations/youtube-oauth.md` con los pasos de Google Cloud, OAuth Desktop y descarga de `client_secret.json`.
3. Implementa `IYouTubeUploader` y un adaptador `YouTubeDryRunUploader` que simule la publicación y guarde metadatos localmente.
4. Configura por defecto `PublishingProvider=DryRun`, `InitialPrivacyStatus=private`, `MadeForKids=true` y `AutoPublishEnabled=false`.
5. No solicitar credenciales reales en esta fase.
6. No realizar una subida real.
```

## Prompt 5 — Levantar Docker Compose local

```text
Levanta el ambiente local completo exclusivamente con Docker Compose.

1. Implementa o valida Dockerfiles multi-stage y `docker-compose.yml`.
2. Servicios mínimos: api, worker, postgres y pgadmin.
3. Incluye FFmpeg dentro de las imágenes que lo necesiten.
4. Usa healthchecks y dependencias por condición saludable.
5. Monta `production/`, `prompts/` y `secrets/` sin copiar secretos a imágenes.
6. Exponer únicamente puertos locales necesarios.
7. Ejecuta `docker compose config`, `docker compose build` y `docker compose up -d`.
8. Ejecuta migraciones desde un contenedor o desde el arranque controlado de la API.
9. Comprueba `/health`, Swagger y conexión a PostgreSQL.
10. Ejecuta un episodio en modo dry-run sin consumir créditos ni publicar.
11. Guarda evidencias en `docs/environment-validation.md`.
```

## Prompt 6 — Integrar la cuenta real de YouTube después del piloto local

```text
Integra la cuenta real de YouTube únicamente después de que el pipeline local haya sido aprobado.

Precondiciones obligatorias:
- Docker Compose local saludable.
- Episodio piloto generado y validado.
- Ensamblaje final correcto.
- Miniatura y subtítulos disponibles.
- Control de calidad aprobado con al menos 85/100.
- Dry-run de publicación exitoso.

Implementación:
1. Configura un proyecto de Google Cloud con YouTube Data API v3.
2. Usa OAuth 2.0 de aplicación de escritorio.
3. Monta `client_secret.json` en el contenedor Worker mediante `./secrets/youtube:/app/secrets/youtube:ro`.
4. Implementa un comando local interactivo para autorizar la cuenta y persistir el token en un volumen local ignorado por Git.
5. Verifica que el canal autorizado sea `Historias de Paolín` antes de subir.
6. Cambia `PublishingProvider` de `DryRun` a `YouTube` solo mediante variable de entorno.
7. Realiza la primera subida como `private`.
8. No publiques automáticamente.
9. Requiere aprobación manual para programar o hacer público el video.
10. Registra videoId, canal, fecha, estado y respuesta de la API sin registrar tokens.
```
