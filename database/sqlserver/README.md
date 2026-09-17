# SQL Server local

Estos scripts se ejecutan contra una instancia SQL Server existente. No crean contenedores ni guardan secretos.

1. `001-create-database.sql`: crea `HistoriasPaolinDb` de forma idempotente.
2. `002-create-login-user.sql`: ejemplos comentados para login, usuario y permisos mínimos de desarrollo.
3. `003-validate-database.sql`: valida acceso, permisos, versión y tabla de migraciones EF Core.

Use cuentas con privilegio mínimo. No conceda `sysadmin` a la aplicación.
