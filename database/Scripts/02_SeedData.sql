-- Makes (Марки авто)
INSERT INTO Makes (Name)
VALUES (N'Toyota'),
       (N'BMW'),
       (N'Tesla'),
       (N'Audi'),
       (N'Mercedes');

-- Suppliers (Постачальники)
INSERT INTO Suppliers (id, CompanyName, ContactName, Phone, Email)
VALUES (1, 'AutoSupplier Ltd', 'John Doe', '123-456-789', 'john@autosupplier.com'),
       (2, 'CarParts Inc', 'Jane Smith', '987-654-321', 'jane@carparts.com'),
       (3, 'Premium Motors', 'Mike Johnson', '555-666-777', 'mike@premiummotors.com');

-- Cars (Автомобілі)
INSERT INTO Cars (id, MakeID, Model, Year, Price, Color, VIN, SupplierID, Status)
VALUES (1, 1, 'Corolla', 2020, 20000.00, 'Red', 'VIN1234567890', 1, 'In stock'),
       (2, 2, 'X5', 2021, 55000.00, 'Black', 'VIN0987654321', 2, 'In stock'),
       (3, 3, 'Model 3', 2022, 40000.00, 'White', 'VIN1122334455', 1, 'In stock'),
       (4, 4, 'A4', 2019, 30000.00, 'Blue', 'VIN5566778899', 3, 'Sold'),
       (5, 5, 'C-Class', 2021, 45000.00, 'Gray', 'VIN6677889900', 2, 'In stock');

-- Customers (Клієнти)
INSERT INTO Customers (id, FullName, Phone, Email, Address)
VALUES (1, 'Michael Johnson', '555-111-222', 'michael@mail.com', '123 Main St'),
       (2, 'Sarah Connor', '555-333-444', 'sarah@mail.com', '456 Oak St'),
       (3, 'James Bond', '555-777-999', 'bond007@mail.com', 'Secret Address');

-- Employees (Співробітники)
INSERT INTO Employees (id, FullName, Position, Phone, Email)
VALUES (1, 'Alice Brown', 'Sales Manager', '555-555-666', 'alice@dealer.com'),
       (2, 'Robert Green', 'Service Advisor', '555-777-888', 'robert@dealer.com'),
       (3, 'Emma White', 'Mechanic', '555-999-000', 'emma@dealer.com');

-- Orders (Замовлення)
INSERT INTO Orders (id, SupplierID, CarID, OrderDate, Quantity, Status)
VALUES (1, 1, 1, '2023-04-10', 5, 'Delivered'),
       (2, 2, 2, '2023-05-20', 2, 'Pending'),
       (3, 3, 4, '2023-06-15', 3, 'Delivered');

-- Sales (Продажі)
INSERT INTO Sales (id, CarID, CustomerID, EmployeeID, SaleDate, FinalPrice)
VALUES (1, 1, 1, 1, '2023-05-01', 19500.00),
       (2, 2, 2, 1, '2023-06-15', 54000.00),
       (3, 4, 3, 2, '2023-07-20', 29000.00);

-- ServiceRequests (Заявки на сервіс)
INSERT INTO ServiceRequests (CarID, ServiceType, Status, UpdatedAt)
VALUES (1, N'Oil Change', 'Completed', '2023-07-01'),
       (2, N'Repainting', 'In Progress', '2023-07-10'),
       (3, N'Tuning', 'Pending', '2023-07-15'),
       (4, N'Wrapping', 'Completed', '2023-07-25'),
       (5, N'Engine Diagnostics', 'Pending', '2023-08-01');
