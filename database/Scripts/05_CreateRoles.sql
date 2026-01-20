USE [auto-dealer-management-system];
GO

-- Адмін
CREATE ROLE db_admin_app;
GO

-- Менеджер
CREATE ROLE db_manager_app;
GO

-- Read-only користувач
CREATE ROLE db_reader_app;
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO db_admin_app;
GRANT EXECUTE TO db_admin_app;

GRANT SELECT, INSERT, UPDATE ON SCHEMA::dbo TO db_manager_app;
GRANT EXECUTE TO db_manager_app;

GRANT SELECT ON SCHEMA::dbo TO db_reader_app;

-- CREATE LOGINS WITH PASSWORDS
-- THEN ->

USE [auto-dealer-management-system];
GO

CREATE USER app_admin FOR LOGIN app_admin;
CREATE USER app_manager FOR LOGIN app_manager;
CREATE USER app_reader FOR LOGIN app_reader;
GO

ALTER ROLE db_admin_app ADD MEMBER app_admin;
ALTER ROLE db_manager_app ADD MEMBER app_manager;
ALTER ROLE db_reader_app ADD MEMBER app_reader;
GO

EXEC sp_helpuser;
EXEC sp_helprolemember 'db_admin_app';


