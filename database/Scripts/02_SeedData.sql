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



-- Makes
INSERT INTO dbo.Makes (Name)
VALUES ('Toyota'), ('Honda'), ('Ford');

-- Suppliers
INSERT INTO dbo.Suppliers (CompanyName, ContactName, Phone, Email)
VALUES
    ('AutoSupply Ltd.', 'John Doe', '123456789', 'john@autosupply.com'),
    ('CarImport Inc.', 'Alice Smith', '987654321', 'alice@carimport.com');

-- Customers
INSERT INTO dbo.Customers (FullName, Phone, Email, Address)
VALUES
    ('Ivan Ivanov', '0501112233', 'ivan@mail.com', 'Kyiv, Ukraine'),
    ('Petro Petrov', '0675558899', 'petro@mail.com', 'Lviv, Ukraine');

-- Employees
INSERT INTO dbo.Employees (FullName, Position, Phone, Email, IsActive)
VALUES
    ('Oleg Koval', 'Sales Manager', '0633334444', 'oleg@dealership.com', 1),
    ('Anna Melnyk', 'Service Manager', '0667778888', 'anna@dealership.com', 1);



INSERT INTO dbo.Cars
(MakeID, Model, Year, Price, Color, VIN, SupplierID, Status, Description, ImageUrl, Condition, Mileage, BodyType)
VALUES
    (1, 'Corolla', 2021, 20000, 'White', 'VIN123COR', 1, 'Sold', 'Reliable compact sedan', 'https://example.com/corolla.jpg', 'New', 0, 'Sedan'),
    (2, 'Civic', 2022, 22000, 'Black', 'VIN456CIV', 2, 'Sold', 'Sporty hatchback with modern features', 'https://example.com/civic.jpg', 'New', 0, 'Hatchback'),
    (3, 'Focus', 2020, 18000, 'Blue', 'VIN789FOC', 1, 'In stock', 'Comfortable family car', 'https://example.com/focus.jpg', 'Used', 15000, 'Sedan');



INSERT INTO dbo.Orders (SupplierID, CarID, OrderDate, Quantity, Status)
VALUES
    (1, 1, GETDATE(), 5, 'Pending'),
    (2, 2, GETDATE(), 3, 'Completed');



INSERT INTO dbo.Sales (CarID, CustomerID, EmployeeID, SaleDate, FinalPrice, Status)
VALUES
    (1, 1, 1, GETDATE(), 21000, 'Completed'),
    (2, 2, 2, GETDATE(), 23000, 'Completed');




INSERT INTO dbo.ServiceRequests (CarID, ServiceType, Status)
VALUES
    (1, 'Oil Change', 'Pending'),
    (2, 'Tire Replacement', 'Completed');
