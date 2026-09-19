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

## Reutilización obligatoria del Portal Corporativo
- `HistoriasPaolin` es una aplicación/dominio consumidor de `https://github.com/christyepez/PortalCorporativo`.
- Antes de crear componentes transversales, leer en PortalCorporativo: `README.md`, `docs/coordination/consumer-onboarding-guide.md`, `codex/REUSABLE_CAPABILITIES.md` y `docs/security/authorization-policy-matrix.md`.
- Reutilizar el API Gateway YARP del portal como punto de entrada.
- Reutilizar Security API para identidad, usuarios, roles, recursos, permisos y autorización.
- Extender Menu API para registrar la navegación de HistoriasPaolin.
- Extender Configuration API para parámetros globales, tenant, módulo y usuario.
- Adaptar Audit API para auditoría append-only y redacción.
- Adaptar Notification API para notificaciones de trabajos, errores, aprobaciones y publicaciones.
- Reutilizar healthchecks, logging estructurado, correlation ID y Seq.
- Reutilizar contratos y patrones SQL Outbox/Inbox, retry, idempotencia y DeadLetter.
- Mantener la base `HistoriasPaolinDb` separada; nunca consultar directamente las bases del portal.
- No duplicar identidad, autorización, menús, configuración, auditoría, notificaciones, gateway, correlation ID ni observabilidad.
- Cuando una capacidad del portal no esté terminada, crear un adaptador contractual en HistoriasPaolin, documentar la dependencia y evitar una implementación paralela definitiva.
- Todo acoplamiento debe hacerse por API, contrato versionado, evento o paquete compartido aprobado; nunca por acceso directo a tablas del portal.
- Clasificar cada capacidad como `REUSE`, `EXTEND`, `ADAPT` o `OWN` y registrar la decisión en documentación de integración.

## Regla de despliegue local
- Todo componente desplegable debe ejecutarse mediante Docker Compose local.
- Servicios mínimos propios en Compose: `historiaspaolin-api`, `historiaspaolin-worker` y `historiaspaolin-migrations`.
- Integrar localmente los servicios necesarios de PortalCorporativo mediante una red Docker compartida o un compose de integración; no copiar su código dentro de HistoriasPaolin.
- SQL Server no se levantará como contenedor porque se utilizará una instancia local ya existente.
- Los contenedores accederán a SQL Server mediante `host.docker.internal` o el host configurable definido en `.env`.
- No usar PostgreSQL, SQLite ni otra base para desarrollo o producción, excepto bases efímeras explícitas para pruebas unitarias aisladas.
- La base de aplicación se llamará por defecto `HistoriasPaolinDb`.
- La creación de la base y las migraciones deben ser idempotentes.
- Las credenciales de SQL Server se proporcionarán únicamente mediante `.env`, variables de entorno o secretos locales fuera de Git.

## Flujo del agente orquestador
1. Leer `docs/SPRINTS.md`.
2. Leer la documentación de reutilización de PortalCorporativo.
3. Seleccionar el primer sprint con estado `PENDIENTE`.
4. Crear rama `sprint/<numero>-<slug>`.
5. Leer los agentes especializados en `.codex/agents/`.
6. Implementar historias en orden.
7. Ejecutar build, tests, linters y validaciones.
8. Actualizar el sprint con evidencias.
9. Crear pull request; no hacer merge automático.

## Definition of Done
- Código compilable.
- Pruebas unitarias y de integración relevantes.
- Docker Compose local operativo cuando aplique.
- Conectividad validada contra la instancia SQL Server existente.
- Base `HistoriasPaolinDb` creada y migrada de forma idempotente.
- Capacidades transversales reutilizadas desde PortalCorporativo o justificadas como `OWN`.
- Sin acceso directo a bases o tablas del portal.
- Documentación y matriz de reutilización actualizadas.
- Sin secretos ni datos personales.
- Logs estructurados y correlation ID compatibles con el portal.
- Manejo de errores, cancelación y reintentos.
- Evidencia de comandos ejecutados.
