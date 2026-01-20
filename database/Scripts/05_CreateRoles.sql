-- 1. Перемикаємось на системну базу master, щоб створити Логіни
USE master;
GO

-- === КОРИСТУВАЧ 1: ПОВНИЙ АДМІН ===
-- Створюємо логін (вхід на сервер)
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'test_admin')
    BEGIN
        CREATE LOGIN test_admin WITH PASSWORD = 'TestPassword123!', CHECK_POLICY = OFF;
    END
GO

-- === КОРИСТУВАЧ 2: МЕНЕДЖЕР (Читання + Запис) ===
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'test_manager')
    BEGIN
        CREATE LOGIN test_manager WITH PASSWORD = 'TestPassword123!', CHECK_POLICY = OFF;
    END
GO

-- === КОРИСТУВАЧ 3: ЧИТАЧ (Тільки читання) ===
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'test_reader')
    BEGIN
        CREATE LOGIN test_reader WITH PASSWORD = 'TestPassword123!', CHECK_POLICY = OFF;
    END
GO

-- 2. Перемикаємось на твою базу даних
-- ВАЖЛИВО: Переконайся, що назва бази правильна
USE [auto-dealer-management-system];
GO

-- === НАЛАШТУВАННЯ test_admin (db_owner) ===
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'test_admin')
    BEGIN
        CREATE USER test_admin FOR LOGIN test_admin;
    END
-- Даємо повні права
ALTER ROLE db_owner ADD MEMBER test_admin;
GO

-- === НАЛАШТУВАННЯ test_manager (db_datareader + db_datawriter) ===
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'test_manager')
    BEGIN
        CREATE USER test_manager FOR LOGIN test_manager;
    END
-- Даємо права на читання та запис даних
ALTER ROLE db_datareader ADD MEMBER test_manager;
ALTER ROLE db_datawriter ADD MEMBER test_manager;
GO

-- === НАЛАШТУВАННЯ test_reader (db_datareader) ===
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'test_reader')
    BEGIN
        CREATE USER test_reader FOR LOGIN test_reader;
    END
-- Даємо права тільки на читання
ALTER ROLE db_datareader ADD MEMBER test_reader;
GO

PRINT 'Користувачі успішно створені!';