SET DATEFORMAT mdy;

-- 1. Create the customer
INSERT INTO Customers (CustomerName) VALUES ('Example Corp');
DECLARE @CustID INT = SCOPE_IDENTITY();

-- 2. Create Products
INSERT INTO Products (ProductName, SKU) VALUES
('Pro Widget', 'PW-1001'),
('Mega Gadget', 'MG-200'),
('Basic Gizmo', 'BG-050'),
('Super Thing', 'ST-45'),
('Hyper Module', 'HM-300');

-- 3. Create Contacts
INSERT INTO Contacts (CustomerID, ContactName, Title) VALUES
(@CustID, 'Jane Doe', 'Head of Purchasing'),
(@CustID, 'John Smith', 'Accounts Payable'),
(@CustID, 'Mike Johnson', 'Receiving Manager');

-- 4. Create Orders
INSERT INTO Orders (OrderReference, CustomerID, OrderDate, TotalAmount, [Status]) VALUES
('#ORD-90012', @CustID, '10/27/2025', 12500.00, 'Processing'),
('#ORD-90011', @CustID, '10/26/2025', 4200.00, 'Shipped'),
('#ORD-90010', @CustID, '10/24/2025', 32000.00, 'Processing'),
('#ORD-90009', @CustID, '10/22/2025', 850.00, 'Delivered');

-- 5. Create OrderLines
INSERT INTO OrderLines (OrderID, ProductID, Quantity, UnitPrice) VALUES
((SELECT OrderID FROM Orders WHERE OrderReference='#ORD-90012'), 2, 980, 12.75),
((SELECT OrderID FROM Orders WHERE OrderReference='#ORD-90011'), 1, 1204, 3.49),
((SELECT OrderID FROM Orders WHERE OrderReference='#ORD-90010'), 5, 430, 74.42),
((SELECT OrderID FROM Orders WHERE OrderReference='#ORD-90009'), 3, 750, 1.13),
((SELECT OrderID FROM Orders WHERE OrderReference='#ORD-90009'), 4, 612, 0.50);

-- 6. Create Invoices
INSERT INTO Invoices (InvoiceReference, CustomerID, OrderID, IssueDate, DueDate, Amount, [Status]) VALUES
('#INV-45001', @CustID, (SELECT OrderID FROM Orders WHERE OrderReference='#ORD-90011'), '10/20/2025', '11/19/2025', 4200.00, 'Paid'),
('#INV-45002', @CustID, (SELECT OrderID FROM Orders WHERE OrderReference='#ORD-90012'), '10/22/2025', '10/25/2025', 12500.00, 'Overdue'),
('#INV-45003', @CustID, NULL, '10/28/2025', '11/27/2025', 3750.00, 'Draft');

-- 7. Create InvoiceLines
INSERT INTO InvoiceLines (InvoiceID, ProductID, Quantity, UnitPrice) VALUES
((SELECT InvoiceID FROM Invoices WHERE InvoiceReference='#INV-45001'), 1, 1204, 3.49),
((SELECT InvoiceID FROM Invoices WHERE InvoiceReference='#INV-45002'), 2, 980, 12.75),
((SELECT InvoiceID FROM Invoices WHERE InvoiceReference='#INV-45003'), 3, 750, 1.13),
((SELECT InvoiceID FROM Invoices WHERE InvoiceReference='#INV-45003'), 5, 25, 74.42);