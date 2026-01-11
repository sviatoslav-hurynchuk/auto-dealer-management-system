-- MAKES
CREATE TABLE dbo.Makes
(
    id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE UNIQUE INDEX UX_Makes_Name
    ON dbo.Makes(Name);


-- SUPPLIERS
CREATE TABLE dbo.Suppliers
(
    id INT IDENTITY PRIMARY KEY,
    CompanyName VARCHAR(100) NOT NULL,
    ContactName VARCHAR(100) NULL,
    Phone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL
);

CREATE UNIQUE INDEX UX_Suppliers_Email
    ON dbo.Suppliers(Email)
    WHERE Email IS NOT NULL;


-- CUSTOMERS
CREATE TABLE dbo.Customers
(
    id INT IDENTITY PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    Address VARCHAR(100) NULL
);

CREATE UNIQUE INDEX UX_Customers_Email
    ON dbo.Customers(Email)
    WHERE Email IS NOT NULL;


-- EMPLOYEES
CREATE TABLE dbo.Employees
(
    id INT IDENTITY PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Position VARCHAR(50) NULL,
    Phone VARCHAR(30) NULL,
    Email VARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE UNIQUE INDEX UX_Employees_Email
    ON dbo.Employees(Email)
    WHERE Email IS NOT NULL;


-- CARS
CREATE TABLE dbo.Cars
(
    id INT IDENTITY PRIMARY KEY,
    MakeID INT NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    Year INT NOT NULL,
    Price DECIMAL(12,2) NOT NULL,
    Color NVARCHAR(30) NULL,
    VIN NVARCHAR(50) NOT NULL,
    SupplierID INT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'In stock',
    Description NVARCHAR(500) NULL,
    ImageUrl NVARCHAR(200) NULL,
    Condition NVARCHAR(50) NULL,
    Mileage INT NULL,
    BodyType NVARCHAR(50) NULL,

    CONSTRAINT FK_Cars_Makes FOREIGN KEY (MakeID) REFERENCES dbo.Makes(id),
    CONSTRAINT FK_Cars_Suppliers FOREIGN KEY (SupplierID) REFERENCES dbo.Suppliers(id)
);

CREATE UNIQUE INDEX UX_Cars_VIN
    ON dbo.Cars(VIN);


-- ORDERS
CREATE TABLE dbo.Orders
(
    id INT IDENTITY PRIMARY KEY,
    SupplierID INT NOT NULL,
    CarID INT NOT NULL,
    OrderDate DATETIME NOT NULL,
    Status VARCHAR(50) NOT NULL,

    CONSTRAINT FK_Orders_Suppliers FOREIGN KEY (SupplierID) REFERENCES dbo.Suppliers(id),
    CONSTRAINT FK_Orders_Cars FOREIGN KEY (CarID) REFERENCES dbo.Cars(id)
);


-- SALES
CREATE TABLE dbo.Sales
(
    id INT IDENTITY PRIMARY KEY,
    CarID INT NOT NULL,
    CustomerID INT NOT NULL,
    EmployeeID INT NOT NULL,
    SaleDate DATETIME NOT NULL,
    FinalPrice DECIMAL(12,2) NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Completed',

    CONSTRAINT FK_Sales_Cars FOREIGN KEY (CarID) REFERENCES dbo.Cars(id),
    CONSTRAINT FK_Sales_Customers FOREIGN KEY (CustomerID) REFERENCES dbo.Customers(id),
    CONSTRAINT FK_Sales_Employees FOREIGN KEY (EmployeeID) REFERENCES dbo.Employees(id)
);


-- SERVICE REQUESTS
CREATE TABLE dbo.ServiceRequests
(
    id INT IDENTITY PRIMARY KEY,
    CarID INT NOT NULL,
    ServiceType NVARCHAR(100) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_ServiceRequests_Cars FOREIGN KEY (CarID) REFERENCES dbo.Cars(id)
);
CREATE INDEX IX_ServiceRequests_CarID ON dbo.ServiceRequests(CarID);

