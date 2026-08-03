# Prompts de implementación

Ejecutar cada prompt en orden. Codex debe leer `AGENTS.md` y `docs/SPRINTS.md` antes de modificar código.

## Prompt maestro de ejecución de sprint

```text
Actúa como agente orquestador de HistoriasPaolin.

1. Lee AGENTS.md, docs/ARCHITECTURE.md y docs/SPRINTS.md.
2. Identifica el primer sprint PENDIENTE.
3. Crea o utiliza la rama `sprint/<numero>-<slug>`.
4. Para cada historia llama conceptualmente al agente especializado adecuado de `.codex/agents/`.
5. Implementa código real, no mocks para las integraciones principales salvo adaptadores de prueba.
6. Después de cada historia ejecuta restore, build y pruebas relacionadas.
7. Corrige fallos antes de continuar.
8. Actualiza checklist, decisiones y evidencias en el sprint.
9. Genera un resumen de archivos modificados, pruebas y riesgos.
10. Crea un pull request en borrador; no hagas merge automático.
11. No consumas créditos ni publiques contenido salvo que el sprint lo exija y exista aprobación explícita.
12. Todo despliegue debe ejecutarse con Docker Compose local.
13. Usa exclusivamente la instancia SQL Server existente y la base `HistoriasPaolinDb`; no agregues PostgreSQL ni un contenedor de base de datos.
```

## Sprint 0 — Bootstrap

```text
Implementa Sprint 0: solución .NET 8, proyectos Api, Worker, Domain, Application, Infrastructure y Contracts; proyectos de pruebas; Directory.Build.props; editorconfig; analyzers; Serilog; Swagger; healthchecks; Dockerfiles; Docker Compose local con servicios api, worker y migrations; configuración tipada y validada; scripts PowerShell de diagnóstico, build, test y arranque. Configura conexión a la instancia SQL Server existente mediante variables de entorno y `host.docker.internal` cuando corresponda. Genera scripts idempotentes para crear `HistoriasPaolinDb`. No levantes una base en Docker. Deja `dotnet build`, `dotnet test` y `docker compose config` exitosos.
```

## Sprint 1 — Dominio y persistencia

```text
Implementa Sprint 1: entidades Episode, EpisodeScene, GenerationJob, MediaAsset, QualityReview, Publication, GenerationCost y ProcessingLog; estados y transiciones válidas; EF Core SQL Server con `Microsoft.EntityFrameworkCore.SqlServer`; repositorios e interfaces; migración inicial; auditoría; concurrencia optimista con `rowversion`; índices y restricciones; pruebas de reglas de dominio y persistencia. Crea `HistoriasPaolinDb` de forma idempotente en la instancia existente y aplica las migraciones desde Docker Compose. No uses PostgreSQL ni SQLite como proveedor funcional de la aplicación.
```

## Sprint 2 — Orquestación idempotente

```text
Implementa Sprint 2: EpisodePipelineOrchestrator reanudable con etapas persistidas en SQL Server, idempotency keys, distributed lock compatible con SQL Server, retries Polly, timeout, cancellation token, compensación y endpoints para crear, consultar, reanudar, aprobar y rechazar episodios. No integrar todavía servicios externos reales.
```

## Sprint 3 — Plantillas y generación narrativa

```text
Implementa Sprint 3: almacenamiento versionado de plantillas en `prompts/`; PromptTemplateService con Scriban; generador de ideas, selector, guion y storyboard; contratos JSON estrictos; validación de contenido infantil; historial antirrepetición persistido en SQL Server; OriginalityService con puntuación explicable; pruebas con casos repetidos y prohibidos.
```

## Sprint 4 — Higgsfield y Seedance

```text
Implementa Sprint 4: IHiggsfieldClient y HiggsfieldCliClient seguros; ProcessStartInfo sin shell; allowlist de modelos; consulta dinámica de esquema y costo; generación de imagen, image-to-video Seedance 2.0, voz y música; parser JSON; descarga y checksums; dry-run; presupuesto diario; reintentos sin duplicar trabajos; pruebas con ejecutable simulado. No consumas créditos durante las pruebas.
```

## Sprint 5 — Producción audiovisual

```text
Implementa Sprint 5: FfmpegService, ffprobe, normalización H.264/AAC, montaje por escenas, mezcla de voz/música, subtítulos SRT, miniatura, episodio 16:9 y Short 9:16; manifiesto de producción; validaciones de duración, codecs, resolución, audio y archivos corruptos; pruebas con fixtures pequeños generados localmente.
```

## Sprint 6 — Calidad y seguridad infantil

```text
Implementa Sprint 6: motor de reglas de calidad, política infantil, continuidad, duplicados, anatomía reportada, propiedad intelectual, umbral de aprobación, bloqueo de publicación, instrucciones específicas de regeneración y dashboard de resultados. Toda publicación debe requerir QualityReview aprobado.
```

## Sprint 7 — YouTube

```text
Implementa Sprint 7 después de validar el piloto completamente local: OAuth Desktop seguro, IYouTubeUploader, subida resumible, metadatos, miniatura, playlists, madeForKids, idioma y programación. Predeterminado privado, PublishingProvider=DryRun y AutoPublishEnabled=false. Implementa dry-run y pruebas del builder. Nunca subas un video real sin aprobación explícita.
```

## Sprint 8 — Automatización recurrente

```text
Implementa Sprint 8: Worker programado en America/Guayaquil, máximo un episodio por ejecución, bloqueo distribuido sobre SQL Server, límites de costo y frecuencia, recuperación tras reinicio, generación diaria configurable, historial, selección no repetitiva y retención de archivos. Añade scripts para Task Scheduler y operación Docker Compose local.
```

## Sprint 9 — Observabilidad y operaciones

```text
Implementa Sprint 9: OpenTelemetry, métricas, tracing, correlation IDs, panel operativo API, alertas por fallos y presupuesto, runbooks, backup de `HistoriasPaolinDb`, limpieza segura de recursos y GitHub Actions para build/test/security scan. No almacenar secretos en Actions. Documenta backup y restauración para la instancia SQL Server existente.
```

## Sprint 10 — Piloto

```text
Ejecuta Sprint 10 en modo piloto local: crea o valida `HistoriasPaolinDb`, genera manifiesto y prompts para un episodio, ejecuta dry-run completo, valida costos y artefactos esperados. Solo con aprobación explícita genera recursos pagados; solo con una segunda aprobación integra OAuth y sube el video como privado. Documenta resultados, costos, fallos y acciones antes de habilitar recurrencia.
```
