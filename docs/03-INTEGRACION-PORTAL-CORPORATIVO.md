# Prompt de integración y reutilización de PortalCorporativo

Usar este prompt en Codex antes de implementar el Sprint 0 o cualquier capacidad transversal.

```text
Actúa como arquitecto de soluciones .NET 8 responsable de integrar HistoriasPaolin con PortalCorporativo.

REPOSITORIOS

Aplicación consumidora:
https://github.com/christyepez/HistoriasPaolin

Plataforma transversal:
https://github.com/christyepez/PortalCorporativo

OBJETIVO

Implementar HistoriasPaolin como una aplicación/dominio consumidor del Portal Corporativo.

No dupliques componentes transversales que ya existan en PortalCorporativo.

HistoriasPaolin debe conservar su dominio, base de datos y pipeline audiovisual propios, pero debe reutilizar seguridad, gateway, navegación, configuración, auditoría, notificaciones, observabilidad e integración transversal del portal.

FASE 1 — INSPECCIÓN OBLIGATORIA

1. Clona o actualiza ambos repositorios en carpetas hermanas:

workspace/
  PortalCorporativo/
  HistoriasPaolin/

2. En PortalCorporativo lee completamente:

- README.md
- AGENTS.md, si existe
- docs/coordination/consumer-onboarding-guide.md
- codex/REUSABLE_CAPABILITIES.md
- docs/security/authorization-policy-matrix.md
- docs/local-development.md
- docker-compose.yml o archivos Compose existentes
- contratos de Gateway, Security, Configuration, Menu, Audit y Notification
- contratos Outbox/Inbox
- configuración de logging, correlation ID, healthchecks y Seq

3. En HistoriasPaolin lee completamente:

- AGENTS.md
- docs/01-LEVANTAR-AMBIENTE.md
- docs/02-PROMPTS-IMPLEMENTACION.md
- docs/SPRINTS.md
- docs/03-INTEGRACION-PORTAL-CORPORATIVO.md
- todos los agentes dentro de `.codex/agents/`

4. No modifiques código hasta terminar el inventario.

5. Genera:

docs/architecture/portal-reuse-matrix.md

La matriz debe contener:

- capacidad;
- componente fuente del portal;
- estado actual;
- clasificación REUSE, EXTEND, ADAPT u OWN;
- forma de integración;
- contrato/API/evento utilizado;
- configuración requerida;
- dependencia;
- riesgo;
- pruebas requeridas;
- responsable.

FASE 2 — CLASIFICACIÓN DE CAPACIDADES

Aplica inicialmente esta clasificación, confirmándola contra el código real:

REUSE

- API Gateway YARP.
- Security API.
- usuarios, roles, recursos y permisos.
- autorización.
- healthchecks.
- logging estructurado.
- correlation ID.
- Seq.

EXTEND

- Menu API.
- Configuration API.
- Workers compartidos cuando el contrato lo permita.
- registro de recursos y permisos.

ADAPT

- Audit API.
- Notification API.
- SQL Outbox/Inbox.
- retry, idempotencia y DeadLetter.

OWN

- dominio de episodios.
- generación de historias y prompts.
- integración Higgsfield.
- Seedance 2.0.
- FFmpeg.
- control de calidad audiovisual.
- YouTube OAuth y publicación.
- almacenamiento de activos de producción.
- base HistoriasPaolinDb.
- Worker de producción audiovisual.

No clasifiques una capacidad como OWN solamente porque resulte más fácil implementarla de nuevo.

FASE 3 — ARQUITECTURA DE INTEGRACIÓN

Diseña la arquitectura para que:

1. El tráfico externo ingrese por el Gateway YARP de PortalCorporativo.
2. El Gateway enrute a HistoriasPaolin.Api.
3. Security API emita o valide la identidad y los permisos.
4. HistoriasPaolin valide autorización basada en recursos y acciones del portal.
5. Menu API exponga el módulo Historias de Paolín y sus opciones.
6. Configuration API administre parámetros configurables del módulo.
7. Audit API reciba eventos de auditoría relevantes.
8. Notification API notifique errores, aprobaciones y publicaciones.
9. Outbox local garantice entrega de eventos sin acoplar transacciones entre bases.
10. Correlation ID viaje desde Gateway hasta API, Worker, SQL, logs y eventos.
11. Seq centralice logs en el ambiente local.
12. Cada repositorio mantenga despliegue y versionado independientes.

No permitas:

- acceso directo de HistoriasPaolin a tablas de PortalCorporativo;
- acceso directo del portal a tablas de HistoriasPaolin;
- claves foráneas entre bases;
- duplicación de usuarios o roles como fuente de verdad;
- duplicación definitiva de menús, permisos, auditoría o notificaciones;
- copiar proyectos completos del portal dentro del repositorio consumidor;
- referencias circulares entre soluciones.

FASE 4 — REGISTRO DEL MÓDULO

Crea documentación y scripts idempotentes para registrar el módulo:

Código del módulo:
HistoriasPaolin

Nombre:
Historias de Paolín

Recursos sugeridos:

- historiaspaolin.dashboard
- historiaspaolin.episodes
- historiaspaolin.scenes
- historiaspaolin.characters
- historiaspaolin.prompts
- historiaspaolin.media
- historiaspaolin.quality
- historiaspaolin.publications
- historiaspaolin.settings
- historiaspaolin.costs
- historiaspaolin.operations

Acciones sugeridas:

- view
- create
- update
- delete
- generate
- approve
- reject
- regenerate
- upload
- schedule
- publish
- cancel
- retry
- configure
- view_costs
- view_logs

Roles sugeridos:

- HistoriasPaolin.Admin
- HistoriasPaolin.Producer
- HistoriasPaolin.Reviewer
- HistoriasPaolin.Publisher
- HistoriasPaolin.Viewer

Genera una matriz rol-permiso conservadora.

No concedas publicación al rol Viewer, Reviewer o Producer.

FASE 5 — MENÚ

Registra en Menu API una estructura similar a:

Historias de Paolín
  Dashboard
  Episodios
  Producción
  Control de calidad
  Publicaciones
  Costos
  Configuración
  Operaciones

Cada opción debe:

- tener recurso y permiso asociado;
- ser filtrada por backend;
- tener orden configurable;
- soportar icono definido por el portal;
- apuntar a rutas del módulo;
- ocultarse cuando el usuario no tenga permiso.

No codifiques el menú estáticamente como única fuente de verdad.

FASE 6 — CONFIGURACIÓN

Reutiliza Configuration API para parámetros no secretos:

- modelos predeterminados;
- duración objetivo;
- cantidad máxima de escenas;
- presupuesto diario;
- máximo de regeneraciones;
- horario de ejecución;
- horario de publicación;
- idioma;
- país;
- madeForKids;
- AutoPublishEnabled;
- RequireHumanApproval;
- umbral de calidad;
- retención de archivos;
- límites de frecuencia.

Mantén fuera de Configuration API los secretos:

- contraseñas SQL Server;
- tokens de YouTube;
- client_secret OAuth;
- tokens Higgsfield;
- claves privadas.

Implementa precedencia compatible con el portal:

global → tenant → module → user

FASE 7 — AUDITORÍA

Adapta Audit API para registrar como mínimo:

- creación de episodio;
- cambio de estado;
- aprobación y rechazo;
- regeneración;
- generación que consume créditos;
- cambio de presupuesto;
- subida a YouTube;
- programación;
- publicación;
- cancelación;
- reintento manual;
- cambio de configuración sensible;
- autenticación OAuth completada o revocada, sin guardar tokens.

Aplicar:

- append-only;
- correlation ID;
- actor;
- timestamp UTC;
- tenant cuando corresponda;
- redacción de secretos;
- before/after solo cuando sea seguro;
- metadatos estructurados.

FASE 8 — NOTIFICACIONES

Adapta Notification API para:

- episodio listo para revisión;
- episodio rechazado;
- trabajo fallido;
- presupuesto excedido;
- OAuth requerido;
- subida privada completada;
- publicación programada;
- publicación completada;
- error de YouTube;
- error de Higgsfield;
- error de FFmpeg.

Usa plantillas e idempotency keys.

En desarrollo local utiliza el proveedor dev del portal.

No envíes notificaciones reales sin configuración explícita.

FASE 9 — OUTBOX/INBOX

Mantén Outbox en HistoriasPaolinDb.

Implementa eventos como:

- EpisodeCreated
- EpisodeReadyForReview
- EpisodeApproved
- EpisodeRejected
- GenerationFailed
- BudgetExceeded
- UploadCompleted
- PublicationScheduled
- EpisodePublished

Reutiliza o adapta los contratos del portal para:

- event ID;
- correlation ID;
- causation ID;
- occurredAtUtc;
- aggregate ID;
- event type;
- version;
- payload;
- retry count;
- next attempt;
- DeadLetter.

No uses transacciones distribuidas entre PortalCorporativo e HistoriasPaolin.

FASE 10 — FRONTEND Y EXPERIENCIA DE PORTAL

Si Angular Shell del portal todavía no está terminado:

1. No construyas un segundo portal corporativo completo.
2. Crea el módulo frontend de HistoriasPaolin desacoplado y preparado para integrarse al shell.
3. Reutiliza tokens visuales, convenciones de rutas, manejo de sesión y contratos definidos por el portal.
4. Documenta el adaptador temporal.
5. Evita duplicar header, sidebar, autenticación, perfil, selector de tenant y menú dinámico como solución definitiva.

Cuando el shell exista:

- cargar HistoriasPaolin como módulo/ruta del portal;
- usar la sesión proporcionada por el portal;
- consumir Menu API;
- aplicar autorización por recurso;
- conservar lazy loading;
- evitar refresh completo al cambiar de menú;
- mantener cabecera y navegación persistentes.

FASE 11 — DOCKER COMPOSE LOCAL

Todo debe funcionar localmente con Docker Compose.

No copies PortalCorporativo dentro de la imagen de HistoriasPaolin.

Crea una estrategia de integración local con uno de estos enfoques, priorizando el menos invasivo:

A. Compose de integración en HistoriasPaolin que consuma imágenes locales etiquetadas del portal.
B. Compose override compartiendo una red Docker externa.
C. Ejecución paralela de ambos Compose en una red externa común.

Red sugerida:

portal-local-network

Servicios esperados del portal cuando correspondan:

- gateway
- security-api
- configuration-api
- menu-api
- audit-api
- notification-api
- portal workers
- seq

Servicios propios:

- historiaspaolin-api
- historiaspaolin-worker
- historiaspaolin-migrations

SQL Server:

- utilizar la instancia existente;
- conectarse con host.docker.internal;
- utilizar HistoriasPaolinDb;
- no crear contenedor SQL Server;
- no compartir tablas con el portal.

Agregar variables públicas de integración a `.env.example`:

PORTAL_GATEWAY_BASE_URL=http://gateway:8080
PORTAL_SECURITY_BASE_URL=http://security-api:<port>
PORTAL_CONFIGURATION_BASE_URL=http://configuration-api:<port>
PORTAL_MENU_BASE_URL=http://menu-api:<port>
PORTAL_AUDIT_BASE_URL=http://audit-api:<port>
PORTAL_NOTIFICATION_BASE_URL=http://notification-api:<port>
PORTAL_MODULE_CODE=HistoriasPaolin
PORTAL_INTEGRATION_ENABLED=true

No inventes puertos: obtén los valores del Compose real del portal.

FASE 12 — CLIENTES Y RESILIENCIA

Implementa clientes tipados:

- IPortalSecurityClient
- IPortalConfigurationClient
- IPortalMenuClient
- IPortalAuditClient
- IPortalNotificationClient

Aplicar:

- HttpClientFactory;
- timeout;
- retry solo para operaciones seguras;
- circuit breaker;
- correlation ID;
- autenticación propagada cuando corresponda;
- manejo explícito de errores;
- logging sin secretos;
- healthcheck por dependencia;
- contratos versionados.

No ocultes una caída del portal.

Define degradación controlada:

- Security no disponible: bloquear operaciones protegidas.
- Configuration no disponible: usar caché válida o configuración local segura documentada.
- Menu no disponible: no afecta procesamiento backend, pero reportar degradación.
- Audit no disponible: persistir evento en Outbox y reintentar.
- Notification no disponible: persistir y reintentar.

FASE 13 — PRUEBAS

Crear pruebas para:

- propagación de correlation ID;
- autorización por recurso/acción;
- registro idempotente del módulo;
- filtrado del menú;
- precedencia de configuración;
- redacción de auditoría;
- notificaciones idempotentes;
- Outbox/Inbox;
- retries y circuit breaker;
- caída de cada dependencia;
- ausencia de acceso directo entre bases;
- Docker Compose integrado;
- smoke test Gateway → HistoriasPaolin.Api → SQL Server;
- smoke de auditoría y notificación.

Usa mocks o servidores HTTP simulados en pruebas unitarias.

Las pruebas de integración reales deben poder activarse con una categoría:

RequiresPortalCorporativo

FASE 14 — DOCUMENTACIÓN

Crear o actualizar:

- docs/architecture/portal-reuse-matrix.md
- docs/architecture/portal-integration.md
- docs/integration/portal-local-compose.md
- docs/integration/module-registration.md
- docs/integration/security-permissions.md
- docs/integration/menu-registration.md
- docs/integration/configuration-keys.md
- docs/integration/audit-events.md
- docs/integration/notification-events.md
- docs/runbooks/portal-dependency-failure.md

Actualizar:

- README.md
- AGENTS.md
- docs/SPRINTS.md
- docs/02-PROMPTS-IMPLEMENTACION.md
- `.env.example`
- diagramas Mermaid

FASE 15 — CAMBIO DEL ROADMAP

Agrega un sprint previo a la implementación funcional:

Sprint 0A — Integración Portal Corporativo

Historias mínimas:

- HP-0A01 Inventario y matriz REUSE/EXTEND/ADAPT/OWN.
- HP-0A02 Integración Gateway y Security.
- HP-0A03 Registro Menu y Configuration.
- HP-0A04 Adaptadores Audit, Notification y Outbox.
- HP-0A05 Compose local integrado y smoke tests.

No iniciar funcionalidades transversales propias hasta completar este sprint.

VALIDACIÓN FINAL

Ejecuta:

- build del PortalCorporativo;
- pruebas del PortalCorporativo relacionadas;
- build de HistoriasPaolin;
- pruebas de HistoriasPaolin;
- docker compose config de ambos entornos;
- compose integrado;
- smoke Gateway → API;
- smoke de autorización;
- smoke de SQL Server;
- smoke de auditoría;
- smoke de notificación.

REGLAS FINALES

- No modificar PortalCorporativo sin crear una rama y PR separado.
- No hacer merge automático en ninguno de los repositorios.
- No romper consumidores existentes del portal.
- No copiar y pegar componentes transversales.
- No compartir bases de datos.
- No guardar secretos.
- No consumir créditos de IA.
- No publicar en YouTube.
- No crear soluciones definitivas duplicadas cuando el portal tenga una capacidad pendiente.

ENTREGABLE

Al finalizar muestra:

1. capacidades reutilizadas;
2. capacidades extendidas;
3. adaptadores creados;
4. capacidades propias justificadas;
5. contratos utilizados;
6. archivos modificados por repositorio;
7. pruebas ejecutadas;
8. resultado del Compose integrado;
9. riesgos y dependencias;
10. pasos manuales pendientes;
11. PR de HistoriasPaolin;
12. PR separado de PortalCorporativo, solo si fue necesario modificarlo.
```
