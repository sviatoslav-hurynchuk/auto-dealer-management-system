INSERT INTO dbo.Makes (Name)
VALUES ('Toyota'),
       ('Honda'),
       ('Ford');


INSERT INTO dbo.Suppliers (CompanyName, ContactName, Phone, Email)
VALUES ('AutoSupply Ltd.', 'John Doe', '123456789', 'john@autosupply.com'),
       ('CarImport Inc.', 'Alice Smith', '987654321', 'alice@carimport.com');


INSERT INTO dbo.Customers (FullName, Phone, Email, Address)
VALUES ('Ivan Ivanov', '0501112233', 'ivan@mail.com', 'Kyiv, Ukraine'),
       ('Petro Petrov', '0675558899', 'petro@mail.com', 'Lviv, Ukraine');


INSERT INTO dbo.Employees (FullName, Position, Phone, Email)
VALUES ('Oleg Koval', 'Sales Manager', '0633334444', 'oleg@dealership.com'),
       ('Anna Melnyk', 'Service Manager', '0667778888', 'anna@dealership.com');


INSERT INTO dbo.Cars (MakeID, Model, Year, Price, Color, VIN, SupplierID, Status)
VALUES (1, 'Corolla', 2021, 20000, 'White', 'VIN123COR', 1, 'In stock'),
       (2, 'Civic', 2022, 22000, 'Black', 'VIN456CIV', 2, 'In stock'),
       (3, 'Focus', 2020, 18000, 'Blue', 'VIN789FOC', 1, 'In stock');


INSERT INTO dbo.Orders (SupplierID, CarID, OrderDate, Quantity, Status)
VALUES (1, 1, GETDATE(), 5, 'Pending'),
       (2, 2, GETDATE(), 3, 'Completed');


INSERT INTO dbo.Sales (CarID, CustomerID, EmployeeID, SaleDate, FinalPrice)
VALUES (1, 1, 1, GETDATE(), 21000),
       (2, 2, 2, GETDATE(), 23000);


INSERT INTO dbo.ServiceRequests (CarID, ServiceType, Status)
VALUES (1, 'Oil Change', 'Pending'),
       (2, 'Tire Replacement', 'Completed');
