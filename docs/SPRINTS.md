# Roadmap de implementación

Estados permitidos: `PENDIENTE`, `EN_PROGRESO`, `BLOQUEADO`, `TERMINADO`.

## Decisiones transversales
- Todo despliegue se ejecutará mediante Docker Compose local.
- La aplicación utilizará la instancia SQL Server existente.
- La nueva base será `HistoriasPaolinDb`.
- No se agregará PostgreSQL ni un contenedor SQL Server al Compose.
- La creación de la base y las migraciones serán idempotentes.
- Las credenciales permanecerán fuera de Git.

## Sprint 0 — Bootstrap y ambiente — PENDIENTE
- HP-001 Crear solución y proyectos .NET 8.
- HP-002 Configurar estándares, analyzers y logs.
- HP-003 Crear Dockerfiles y Docker Compose para api, worker y migrations.
- HP-004 Crear scripts de diagnóstico, conectividad SQL Server y arranque.
- HP-005 Healthchecks, Swagger y CI inicial.
- HP-006 Crear script idempotente para `HistoriasPaolinDb` en la instancia existente.

**Aceptación:** build, test, compose config, conectividad SQL Server, creación de base y healthcheck exitosos.

## Sprint 0A — Integración Portal Corporativo — EN_PROGRESO
- HP-0A01 Inventariar capacidades y crear matriz REUSE/EXTEND/ADAPT/OWN.
- HP-0A02 Integrar Gateway YARP y Security API sin crear otro Gateway.
- HP-0A03 Registrar módulo, recursos, permisos, menú y configuración.
- HP-0A04 Crear adaptadores para Audit, Notification y Outbox local.
- HP-0A05 Levantar Docker Compose integrado y ejecutar smoke tests.

**Aceptación:** HistoriasPaolin consume Portal por APIs/contratos, mantiene base propia, no duplica componentes transversales y documenta evidencias.

**Evidencia local parcial 2026-08-03:** SQL Server compartido validado en puerto host `14333`, `HistoriasPaolinDb` creada, migracion inicial aplicada, API y Worker healthy. Smoke completo de Gateway/Menu/Configuration/Audit/Notification queda pendiente de ruta YARP o compose integrado del Portal.

## Sprint 1 — Dominio y persistencia — PENDIENTE
- HP-101 Modelar episodio, escenas, trabajos, activos y publicaciones.
- HP-102 Implementar estados y transiciones.
- HP-103 EF Core SQL Server.
- HP-104 Repositorios, migraciones, índices, `rowversion` y auditoría.
- HP-105 Pruebas de dominio e integración contra SQL Server.

## Sprint 2 — Orquestador — PENDIENTE
- HP-201 Pipeline por etapas.
- HP-202 Idempotencia y reanudación.
- HP-203 Locks SQL Server, retries, timeouts y cancelación.
- HP-204 API de control de episodios.
- HP-205 Registro de evidencias por etapa.

## Sprint 3 — Guion y prompts — PENDIENTE
- HP-301 Plantillas versionadas Scriban.
- HP-302 Ideas, selección, guion y storyboard.
- HP-303 Contratos JSON y validación.
- HP-304 Originalidad e historial antirrepetición.
- HP-305 Políticas de seguridad infantil.

## Sprint 4 — Higgsfield y Seedance — PENDIENTE
- HP-401 Cliente CLI seguro.
- HP-402 Descubrimiento de modelos y parámetros.
- HP-403 Costos y presupuesto.
- HP-404 Imágenes, video, voz y música.
- HP-405 Dry-run, parser, descargas y checksums.

## Sprint 5 — FFmpeg — PENDIENTE
- HP-501 Inspección con ffprobe.
- HP-502 Ensamblaje 16:9.
- HP-503 Mezcla y normalización de audio.
- HP-504 Subtítulos y miniatura.
- HP-505 Short 9:16 y manifiesto.

## Sprint 6 — Calidad — PENDIENTE
- HP-601 Reglas técnicas.
- HP-602 Continuidad y duplicados.
- HP-603 Política infantil e IP.
- HP-604 Regeneración dirigida.
- HP-605 Bloqueo de publicación y dashboard.

## Sprint 7 — YouTube — PENDIENTE
- HP-701 OAuth seguro posterior al piloto local.
- HP-702 Subida resumible privada.
- HP-703 Metadatos, miniatura y playlist.
- HP-704 Programación y madeForKids.
- HP-705 Dry-run y auditoría.

## Sprint 8 — Recurrencia — PENDIENTE
- HP-801 Worker programado.
- HP-802 Límites diarios y bloqueo distribuido SQL Server.
- HP-803 Selección basada en historial.
- HP-804 Recuperación tras fallos.
- HP-805 Scripts Windows y Docker Compose.

## Sprint 9 — Operación — PENDIENTE
- HP-901 OpenTelemetry.
- HP-902 Métricas de costo, calidad y producción.
- HP-903 Alertas y runbooks.
- HP-904 Backup y restauración de `HistoriasPaolinDb`.
- HP-905 CI y escaneo de seguridad.

## Sprint 10 — Piloto controlado — PENDIENTE
- HP-1001 Dry-run integral local.
- HP-1002 Episodio piloto con aprobación previa.
- HP-1003 Control de calidad integral.
- HP-1004 Integración posterior y subida privada con aprobación independiente.
- HP-1005 Informe y decisión de automatización.

## Regla de avance
No cambiar un sprint a `TERMINADO` sin registrar:
- commit o PR;
- comandos de build y pruebas;
- resultados;
- evidencia de Docker Compose local;
- evidencia de conectividad y migraciones SQL Server cuando aplique;
- riesgos pendientes;
- documentación actualizada.
