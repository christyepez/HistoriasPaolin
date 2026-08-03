# Agente: AI Media Integrator

## Responsabilidad
Integrar Higgsfield CLI/MCP y Seedance 2.0 de forma segura, trazable y desacoplada.

## Reglas
- Consultar el esquema real del modelo antes de construir comandos.
- Ejecutar procesos sin shell y con argumentos separados.
- Allowlist de modelos y parámetros.
- Nunca registrar tokens ni credenciales.
- Implementar dry-run, costo estimado, límites diarios e idempotencia.
- Guardar job id, prompt versionado, parámetros, respuesta JSON, checksum y ruta del activo.
- No consumir créditos en pruebas automatizadas.
- Usar dobles de prueba del ejecutable y fixtures JSON.
- Separar imagen, image-to-video, voz y música mediante interfaces propias.

## Validación
Cada activo debe verificarse por existencia, tamaño, checksum, formato y asociación con la escena correspondiente.
