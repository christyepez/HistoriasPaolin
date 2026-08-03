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

## Regla de despliegue local
- Todo componente desplegable debe ejecutarse mediante Docker Compose local.
- Servicios mínimos en Compose: `historiaspaolin-api`, `historiaspaolin-worker` y `historiaspaolin-migrations`.
- HistoriasPaolin debe consumir capacidades transversales del PortalCorporativo; no duplicar Gateway, seguridad, usuarios, roles, permisos, menu, configuracion, auditoria ni notificaciones.
- La integracion con PortalCorporativo se realiza por APIs, Gateway, contratos HTTP y eventos/outbox local; nunca por acceso directo a tablas.
- SQL Server no se levantará como contenedor porque se utilizará una instancia local ya existente.
- Los contenedores accederán a SQL Server mediante `host.docker.internal` o el host configurable definido en `.env`.
- No usar PostgreSQL, SQLite ni otra base para desarrollo o producción, excepto bases efímeras explícitas para pruebas unitarias aisladas.
- La base de aplicación se llamará por defecto `HistoriasPaolinDb`.
- La creación de la base y las migraciones deben ser idempotentes.
- Las credenciales de SQL Server se proporcionarán únicamente mediante `.env`, variables de entorno o secretos locales fuera de Git.

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
- Docker Compose local operativo cuando aplique.
- Conectividad validada contra la instancia SQL Server existente.
- Base `HistoriasPaolinDb` creada y migrada de forma idempotente.
- Matriz de reutilizacion PortalCorporativo actualizada cuando se agregue una capacidad transversal.
- Documentación actualizada.
- Sin secretos ni datos personales.
- Logs estructurados.
- Manejo de errores, cancelación y reintentos.
- Evidencia de comandos ejecutados.
