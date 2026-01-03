CREATE TABLE dbo.Makes
(
    id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) UNIQUE NOT NULL
);


CREATE TABLE dbo.Suppliers
(
    id INT IDENTITY PRIMARY KEY,
    CompanyName VARCHAR(100) NOT NULL,
    ContactName VARCHAR(100) NULL,
    Phone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL
);


CREATE TABLE dbo.Customers
(
    id INT IDENTITY PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    Address VARCHAR(100) NULL
);


CREATE TABLE dbo.Employees
(
    id INT IDENTITY PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Position VARCHAR(50) NULL,
    Phone VARCHAR(30) NULL,
    Email VARCHAR(100) NULL
);


CREATE TABLE dbo.Cars
(
    id INT IDENTITY PRIMARY KEY,
    MakeID INT NOT NULL,
    Model VARCHAR(50) NOT NULL,
    Year INT NOT NULL,
    Price DECIMAL(12,2) NOT NULL,
    Color VARCHAR(30) NULL,
    VIN VARCHAR(50) UNIQUE NOT NULL,
    SupplierID INT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'In stock',
    CONSTRAINT FK_Cars_Makes FOREIGN KEY (MakeID) REFERENCES dbo.Makes(id),
    CONSTRAINT FK_Cars_Suppliers FOREIGN KEY (SupplierID) REFERENCES dbo.Suppliers(id)
);


CREATE TABLE dbo.Orders
(
    id INT IDENTITY PRIMARY KEY,
    SupplierID INT NOT NULL,
    CarID INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    Quantity INT NOT NULL,
    Status VARCHAR(50) NOT NULL,
    CONSTRAINT FK_Orders_Suppliers FOREIGN KEY (SupplierID) REFERENCES dbo.Suppliers(id),
    CONSTRAINT FK_Orders_Cars FOREIGN KEY (CarID) REFERENCES dbo.Cars(id)
);


CREATE TABLE dbo.Sales
(
    id INT IDENTITY PRIMARY KEY,
    CarID INT NOT NULL,
    CustomerID INT NOT NULL,
    EmployeeID INT NOT NULL,
    SaleDate DATETIME NOT NULL,
    FinalPrice DECIMAL(12,2) NOT NULL,
    CONSTRAINT FK_Sales_Cars FOREIGN KEY (CarID) REFERENCES dbo.Cars(id),
    CONSTRAINT FK_Sales_Customers FOREIGN KEY (CustomerID) REFERENCES dbo.Customers(id),
    CONSTRAINT FK_Sales_Employees FOREIGN KEY (EmployeeID) REFERENCES dbo.Employees(id)
);


CREATE TABLE dbo.ServiceRequests
(
    id INT IDENTITY PRIMARY KEY,
    CarID INT NOT NULL,
    ServiceType NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_ServiceRequests_Cars FOREIGN KEY (CarID) REFERENCES dbo.Cars(id)
);
