/*
Development-only examples. Do not store real passwords in this file.
Run manually as an administrator of the local SQL Server instance when the
application account does not already exist.

CREATE LOGIN [historiaspaolin_app] WITH PASSWORD = '<use-a-local-secret>';
GO

USE [HistoriasPaolinDb];
GO

CREATE USER [historiaspaolin_app] FOR LOGIN [historiaspaolin_app];
GO

-- Minimum local development permissions for EF Core migrations.
ALTER ROLE [db_datareader] ADD MEMBER [historiaspaolin_app];
ALTER ROLE [db_datawriter] ADD MEMBER [historiaspaolin_app];
ALTER ROLE [db_ddladmin] ADD MEMBER [historiaspaolin_app];
GO

-- Avoid sysadmin. For stricter setups, create a separate migration account
-- with db_ddladmin and run the application with datareader/datawriter only.
*/
