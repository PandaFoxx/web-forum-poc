IF NOT EXISTS (SELECT * FROM sys.databases WHERE [name] = 'iidentifii-web-forum')
BEGIN
	CREATE DATABASE [iidentifii-web-forum];
END
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE [name] = 'iidentifii-user')
BEGIN
	CREATE LOGIN [iidentifii-user] WITH PASSWORD = ''; -- TODO enter password manually or inject from pipeline
END
GO

USE [iidentifii-web-forum];

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE [name] = 'iidentifii-user')
BEGIN
	CREATE USER [iidentifii-user] FOR LOGIN [iidentifii-user];
	EXEC sp_addrolemember 'db_owner', 'iidentifii-user';
END
GO
