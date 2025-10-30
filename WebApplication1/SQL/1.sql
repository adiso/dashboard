-- 1. Customers
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(255) NOT NULL
);

-- 2. Products
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(255) NOT NULL,
    SKU NVARCHAR(100) NOT NULL
);

-- 3. Contacts
CREATE TABLE Contacts (
    ContactID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    ContactName NVARCHAR(255) NOT NULL,
    Title NVARCHAR(100) NOT NULL
);

-- 4. Orders
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    OrderReference NVARCHAR(50) UNIQUE NOT NULL,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    OrderDate DATETIME NOT NULL,
    TotalAmount DECIMAL(18, 2) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL
);

-- 5. OrderLines
CREATE TABLE OrderLines (
    OrderLineID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL
);

-- 6. Invoices
CREATE TABLE Invoices (
    InvoiceID INT PRIMARY KEY IDENTITY(1,1),
    InvoiceReference NVARCHAR(50) UNIQUE NOT NULL,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    OrderID INT NULL FOREIGN KEY REFERENCES Orders(OrderID),
    IssueDate DATETIME NOT NULL,
    DueDate DATETIME NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL
);

-- 7. InvoiceLines
CREATE TABLE InvoiceLines (
    InvoiceLineID INT PRIMARY KEY IDENTITY(1,1),
    InvoiceID INT FOREIGN KEY REFERENCES Invoices(InvoiceID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL
);