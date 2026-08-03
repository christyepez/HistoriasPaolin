# AGENTS.md — Historias de Paolín

## Misión
Construir una plataforma .NET 8 que genere, valide, ensamble y publique episodios infantiles originales usando Higgsfield, Seedance 2.0, FFmpeg y YouTube Data API.

## Reglas obligatorias
- Aplicar arquitectura vertical por funcionalidad.
- Para API REST usar Controllers, DTO, interfaces, servicios y repositorios.
- Un sprint se implementa completo antes de iniciar el siguiente.
- Compilar y ejecutar pruebas después de cada historia.
- No consumir créditos ni publicar videos sin una orden explícita o sin `AutoPublishEnabled=true`.
- Toda publicación inicial será privada.
- No guardar secretos, tokens, videos generados ni archivos OAuth en Git.
- Todo proceso debe ser idempotente, reanudable y auditable.
- Máximo un episodio por ejecución.
- No imitar franquicias, personajes, voces, canciones o marcas existentes.

## Política obligatoria de despliegue local
- Todo componente que necesite desplegarse debe ejecutarse en Docker Compose local.
- No desplegar en Azure, AWS, Google Cloud, Kubernetes, servidores externos, hosting administrado ni servicios PaaS.
- Los servicios API, Worker, PostgreSQL, pgAdmin y cualquier servicio auxiliar deben declararse en `docker-compose.yml` o archivos Compose complementarios.
- Higgsfield, Seedance y YouTube se consumen como integraciones externas desde los contenedores; sus credenciales deben montarse mediante secretos o volúmenes locales y nunca incluirse en las imágenes.
- Los Dockerfiles deben soportar compilación reproducible multi-stage y ejecución sin depender del SDK instalado en el host.
- Usar healthchecks, redes privadas, volúmenes persistentes y dependencias condicionadas por salud.
- Exponer únicamente los puertos necesarios hacia `localhost`.
- Toda historia que agregue infraestructura debe actualizar Docker Compose, `.env.example`, documentación y pruebas de arranque.
- La validación mínima de despliegue será: `docker compose config`, `docker compose build`, `docker compose up -d`, comprobación de healthchecks y `docker compose down`.

## Flujo del agente orquestador
1. Leer `docs/SPRINTS.md`.
2. Seleccionar el primer sprint con estado `PENDIENTE`.
3. Crear rama `sprint/<numero>-<slug>`.
4. Leer los agentes especializados en `.codex/agents/`.
5. Implementar historias en orden.
6. Ejecutar build, tests, linters y validaciones.
7. Actualizar el sprint con evidencias.
8. Crear pull request; no hacer merge automático.

## Definition of Done
- Código compilable.
- Pruebas unitarias y de integración relevantes.
- Docker Compose local operativo para todo componente desplegable.
- `docker compose config` y `docker compose build` sin errores.
- Servicios levantados localmente con healthchecks saludables.
- Documentación actualizada.
- Sin secretos ni datos personales.
- Logs estructurados.
- Manejo de errores, cancelación y reintentos.
- Evidencia de comandos ejecutados.
