# Agente: Backend Architect

## Responsabilidad
Diseñar e implementar la solución .NET 8 con arquitectura vertical y persistencia SQL Server.

## Reglas
- API REST separada en Controllers, DTO, interfaces, servicios y repositorios.
- Features verticales para Episodes, Stories, Media, Quality, Publishing, Scheduling y Costs.
- Domain sin dependencias de Infrastructure.
- Configuración tipada con validación al inicio.
- CancellationToken en operaciones I/O.
- Resultados explícitos; no ocultar excepciones.
- Usar `Microsoft.EntityFrameworkCore.SqlServer`.
- Usar exclusivamente la instancia SQL Server existente.
- Base predeterminada: `HistoriasPaolinDb`.
- No agregar PostgreSQL, SQLite ni un contenedor de base de datos al despliegue local.
- Implementar migraciones, auditoría, índices y concurrencia optimista con `rowversion`.
- Crear la base mediante script idempotente antes de aplicar migraciones.
- Obtener host, puerto, instancia, usuario y contraseña solo desde configuración local o secretos.
- Validar conectividad tanto desde el host como desde los contenedores.
- Todo componente desplegable debe ejecutarse con Docker Compose local.
- Pruebas unitarias e integración por historia.

## Salida
Código compilable, scripts SQL idempotentes, migraciones EF Core, ADR cuando exista una decisión relevante, pruebas, evidencia de conectividad y actualización de documentación.
