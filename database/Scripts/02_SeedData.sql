DELETE FROM dbo.ServiceRequests;
DELETE FROM dbo.Sales;
DELETE FROM dbo.Orders;
DELETE FROM dbo.Cars;
DELETE FROM dbo.Customers;
DELETE FROM dbo.Employees;
DELETE FROM dbo.Suppliers;
DELETE FROM dbo.Makes;

DBCC CHECKIDENT ('dbo.Makes', RESEED, 0);
DBCC CHECKIDENT ('dbo.Suppliers', RESEED, 0);
DBCC CHECKIDENT ('dbo.Customers', RESEED, 0);
DBCC CHECKIDENT ('dbo.Employees', RESEED, 0);
DBCC CHECKIDENT ('dbo.Cars', RESEED, 0);
DBCC CHECKIDENT ('dbo.Orders', RESEED, 0);
DBCC CHECKIDENT ('dbo.Sales', RESEED, 0);
DBCC CHECKIDENT ('dbo.ServiceRequests', RESEED, 0);

-- ======================
-- MAKES
-- ======================
INSERT INTO dbo.Makes (Name)
VALUES
    ('Toyota'),
    ('Honda'),
    ('Ford'),
    ('BMW'),
    ('Audi'),
    ('Volkswagen'),
    ('Hyundai'),
    ('Kia');

-- ======================
-- SUPPLIERS
-- ======================
INSERT INTO dbo.Suppliers (CompanyName, ContactName, Phone, Email)
VALUES
    ('AutoSupply Ltd.', 'John Doe', '123456789', 'john@autosupply.com'),
    ('CarImport Inc.', 'Alice Smith', '987654321', 'alice@carimport.com'),
    ('EuroCars GmbH', 'Hans Müller', '111222333', 'hans@eurocars.de'),
    ('AsiaMotors', 'Lee Wong', '444555666', 'lee@asiamotors.com');

-- ======================
-- CUSTOMERS
-- ======================
INSERT INTO dbo.Customers (FullName, Phone, Email, Address)
VALUES
    ('Ivan Ivanov', '0501112233', 'ivan@mail.com', 'Kyiv'),
    ('Petro Petrov', '0675558899', 'petro@mail.com', 'Lviv'),
    ('Andrii Shevchenko', '0632223344', 'andrii@mail.com', 'Odesa'),
    ('Mykola Bondar', '0958887766', 'mykola@mail.com', 'Dnipro'),
    ('Oleksii Tkachenko', '0984443322', 'oleksii@mail.com', 'Kharkiv');

-- ======================
-- EMPLOYEES
-- ======================
INSERT INTO dbo.Employees (FullName, Position, Phone, Email, IsActive)
VALUES
    ('Oleg Koval', 'Sales Manager', '0633334444', 'oleg@dealership.com', 1),
    ('Anna Melnyk', 'Service Manager', '0667778888', 'anna@dealership.com', 1),
    ('Dmytro Horbunov', 'Sales Manager', '0971112233', 'dmytro@dealership.com', 1);

-- ======================
-- CARS (BIG DATASET)
-- ======================
INSERT INTO dbo.Cars
(MakeID, Model, Year, Price, Color, VIN, SupplierID, Status, Description, ImageUrl, Condition, Mileage, BodyType)
VALUES
-- Toyota
(1, 'Corolla', 2019, 15000, 'White', 'VIN-T-001', 1, 'Sold', 'Compact sedan', NULL, 'Used', 40000, 'Sedan'),
(1, 'Corolla', 2022, 21000, 'Black', 'VIN-T-002', 2, 'In stock', 'New Corolla', NULL, 'New', 0, 'Sedan'),
(1, 'Camry', 2021, 26000, 'Gray', 'VIN-T-003', 3, 'In stock', 'Business sedan', NULL, 'Used', 20000, 'Sedan'),

-- Honda
(2, 'Civic', 2018, 14000, 'Blue', 'VIN-H-001', 1, 'Sold', 'Reliable car', NULL, 'Used', 60000, 'Sedan'),
(2, 'Civic', 2023, 24000, 'Red', 'VIN-H-002', 2, 'In stock', 'Brand new Civic', NULL, 'New', 0, 'Sedan'),
(2, 'Accord', 2020, 23000, 'Black', 'VIN-H-003', 2, 'Archived', 'Comfort sedan', NULL, 'Used', 30000, 'Sedan'),

-- Ford
(3, 'Focus', 2017, 12000, 'Silver', 'VIN-F-001', 1, 'Sold', 'Budget hatchback', NULL, 'Used', 80000, 'Hatchback'),
(3, 'Focus', 2020, 17000, 'White', 'VIN-F-002', 4, 'In stock', 'Family car', NULL, 'Used', 25000, 'Hatchback'),
(3, 'Mustang', 2022, 45000, 'Yellow', 'VIN-F-003', 3, 'In stock', 'Sport car', NULL, 'New', 0, 'Coupe'),

-- BMW
(4, 'X5', 2019, 52000, 'Black', 'VIN-B-001', 3, 'Sold', 'Luxury SUV', NULL, 'Used', 30000, 'SUV'),
(4, '3 Series', 2021, 41000, 'Blue', 'VIN-B-002', 3, 'In stock', 'Sport sedan', NULL, 'Used', 20000, 'Sedan'),

-- Audi
(5, 'A4', 2020, 39000, 'Gray', 'VIN-A-001', 4, 'In stock', 'Premium sedan', NULL, 'Used', 25000, 'Sedan'),
(5, 'Q7', 2022, 65000, 'White', 'VIN-A-002', 4, 'In stock', 'Luxury SUV', NULL, 'New', 0, 'SUV');

-- ======================
-- ORDERS
-- ======================
INSERT INTO dbo.Orders (SupplierID, CarID, OrderDate, Status)
VALUES
    (1, 2, DATEADD(day, -10, GETDATE()), 'Completed'),
    (2, 5, DATEADD(day, -5, GETDATE()), 'Completed'),
    (3, 10, DATEADD(day, -3, GETDATE()), 'Pending');

-- ======================
-- SALES
-- ======================
INSERT INTO dbo.Sales (CarID, CustomerID, EmployeeID, SaleDate, FinalPrice, Status)
VALUES
    (1, 1, 1, DATEADD(month, -6, GETDATE()), 15500, 'Completed'),
    (4, 2, 2, DATEADD(month, -3, GETDATE()), 14500, 'Completed'),
    (10, 3, 1, DATEADD(month, -1, GETDATE()), 51000, 'Completed');

-- ======================
-- SERVICE REQUESTS
-- ======================
INSERT INTO dbo.ServiceRequests (CarID, ServiceType, Status)
VALUES
    (1, 'Oil Change', 'Completed'),
    (4, 'Brake Check', 'Pending'),
    (10, 'Full Inspection', 'Completed');
