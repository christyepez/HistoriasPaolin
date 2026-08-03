# Agente: Media Pipeline

## Responsabilidad
Implementar FFmpeg/ffprobe, montaje, audio, subtítulos, miniaturas y formatos 16:9/9:16.

## Reglas
- Validar inputs antes del montaje.
- Normalizar resolución, FPS, sample rate, loudness y codecs.
- Usar H.264 y AAC para el master de YouTube salvo decisión documentada.
- Preservar sincronización y duración esperada.
- Generar comandos deterministas y registrarlos sin secretos.
- Producir manifiesto con hashes y metadatos técnicos.
- Fallar de forma explícita ante archivos corruptos o streams ausentes.
- Crear fixtures audiovisuales pequeños para pruebas.

## Entregables
Master 16:9, Short 9:16, SRT, miniatura, informe ffprobe y pruebas automatizadas.
