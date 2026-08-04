# SQL Server local setup

1. Identifique instancia: SQL Server Configuration Manager o `sqlcmd -L`.
2. Habilite TCP/IP para la instancia y reinicie el servicio SQL Server.
3. Valide puerto fijo, usualmente `1433`, en SQL Server Network Configuration.
4. Si usa autenticacion SQL, habilite modo mixto y cree login sin `sysadmin`.
5. Cree `HistoriasPaolinDb` con `scripts/initialize-sqlserver.ps1`.
6. Desde Docker use `host.docker.internal`; en Linux Compose agrega `host-gateway`.
7. Para certificados locales use `SQLSERVER_TRUST_SERVER_CERTIFICATE=true`; en ambientes no locales configure certificados confiables.
8. Error de login: revise usuario, password, base predeterminada y permisos `db_datareader`, `db_datawriter`, `db_ddladmin` para migraciones locales.
9. Error de red: revise firewall, TCP/IP, puerto, instancia nombrada y SQL Browser.
10. Aplique migraciones con `scripts/apply-migrations.ps1`.
11. Respaldo: `BACKUP DATABASE [HistoriasPaolinDb] TO DISK = '<ruta>.bak'`.
12. Restauracion: use `RESTORE DATABASE` solo sobre bases locales revisadas.
13. Healthcheck: `GET /health/ready` debe ser healthy cuando SQL Server este disponible.
14. Ambiente completo: levantar PortalCorporativo, completar `.env`, ejecutar `docker compose up -d`.

## Validacion local 2026-08-03

- Puerto host usado: `14333`.
- Desde Windows se valido con `localhost,14333`.
- Desde contenedores se valido con `host.docker.internal,14333`.
- Base creada: `HistoriasPaolinDb`.
- Login dedicado: `historiaspaolin_app`.
- Migracion aplicada: `20260803220000_InitialSqlServerSchema`.
- No se documento ningun secreto.
