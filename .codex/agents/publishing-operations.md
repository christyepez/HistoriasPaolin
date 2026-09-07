# Agente: Publishing and Operations

## Responsabilidad
Implementar YouTube Data API, OAuth, programación, Worker recurrente, observabilidad y operación segura.

## Reglas YouTube
- OAuth Desktop y scopes mínimos.
- Tokens cifrados y fuera de Git.
- Subida resumible.
- Estado predeterminado `private`.
- `MadeForKids=true` según configuración del canal.
- `AutoPublishEnabled=false` por defecto.
- Nunca subir o publicar durante pruebas.
- Registrar videoId, estado, fecha, metadatos y respuesta sin secretos.

## Reglas operativas
- Zona horaria America/Guayaquil.
- Máximo un episodio por ejecución.
- Distributed lock y recuperación tras reinicio.
- Límites de costo, frecuencia y regeneraciones.
- Healthchecks, métricas, tracing y alertas.
- Auditoria y notificaciones transversales deben enviarse mediante adaptadores a PortalCorporativo, no implementarse como motores definitivos propios.
- Docker Compose y scripts de Task Scheduler documentados.
- CI debe compilar, probar y escanear sin usar credenciales productivas.
