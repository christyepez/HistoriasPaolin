SET NOCOUNT ON;

SELECT
    DB_ID(N'HistoriasPaolinDb') AS HistoriasPaolinDbId,
    SUSER_SNAME() AS LoginName,
    USER_NAME() AS DatabaseUser,
    @@VERSION AS SqlServerVersion,
    DB_NAME() AS CurrentDatabase,
    HAS_DBACCESS(N'HistoriasPaolinDb') AS HasDatabaseAccess,
    HAS_PERMS_BY_NAME(N'HistoriasPaolinDb', 'DATABASE', 'CONNECT') AS CanConnectDatabase;

IF DB_ID(N'HistoriasPaolinDb') IS NOT NULL
BEGIN
    EXEC(N'USE [HistoriasPaolinDb];
    SELECT
        DB_NAME() AS ValidatedDatabase,
        OBJECT_ID(N''[dbo].[__EFMigrationsHistory]'') AS EfMigrationsHistoryObjectId,
        HAS_PERMS_BY_NAME(DB_NAME(), ''DATABASE'', ''CREATE TABLE'') AS CanCreateTable,
        HAS_PERMS_BY_NAME(DB_NAME(), ''DATABASE'', ''ALTER'') AS CanAlterDatabase;');
END;
GO
