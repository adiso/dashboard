

SET DATEFORMAT mdy;
DECLARE @CustID INT = 1; -- Assuming the customer ID is 1
DECLARE @Counter INT = 1;
DECLARE @NewInvoiceRef NVARCHAR(50);
DECLARE @NewInvoiceID INT;
DECLARE @IssueDate DATETIME;
DECLARE @DueDate DATETIME;
DECLARE @Amount DECIMAL(18, 2);
DECLARE @Status NVARCHAR(50);

PRINT 'Generating 25 new invoices...';

WHILE @Counter <= 25
BEGIN
    -- 1. Generate Invoice Data
    SET @NewInvoiceRef = '#INV-452' + FORMAT(@Counter + 3, '000'); -- Start after #INV-45003
    SET @IssueDate = DATEADD(day, -(@Counter * 14), GETDATE()); -- Go back in 14-day intervals
    SET @DueDate = DATEADD(day, 30, @IssueDate);
    SET @Amount = 0; -- We will calculate this from lines later
    
    -- 2. Determine Status
    IF (RAND() < 0.6)
        SET @Status = 'Paid';
    ELSE IF (RAND() < 0.8 AND @DueDate < GETDATE())
        SET @Status = 'Overdue';
    ELSE
        SET @Status = 'Draft';

    -- Make 'Overdue' logical
    IF @Status = 'Overdue'
        SET @DueDate = DATEADD(day, -5, GETDATE()); -- Make it due 5 days ago

    -- 3. Insert Invoice Header
    INSERT INTO Invoices (InvoiceReference, CustomerID, OrderID, IssueDate, DueDate, Amount, [Status])
    VALUES (@NewInvoiceRef, @CustID, NULL, @IssueDate, @DueDate, @Amount, @Status);

    SET @NewInvoiceID = SCOPE_IDENTITY();

    -- 4. Insert Invoice Lines (1 to 3 lines per invoice)
    DECLARE @LineCounter INT = 1;
    DECLARE @NumLines INT = FLOOR(RAND() * 3) + 1; -- 1, 2, or 3 lines
    DECLARE @ProductID INT;
    DECLARE @Quantity INT;
    DECLARE @UnitPrice DECIMAL(18, 2);

    WHILE @LineCounter <= @NumLines
    BEGIN
        SET @ProductID = FLOOR(RAND() * 5) + 1; -- Product IDs 1 to 5
        SET @Quantity = FLOOR(RAND() * 20) + 1; -- 1 to 21 quantity
        
        -- Get the unit price from the Products table
        SELECT @UnitPrice = (
            CASE @ProductID
                WHEN 1 THEN 3.49
                WHEN 2 THEN 12.75
                WHEN 3 THEN 1.13
                WHEN 4 THEN 0.50
                WHEN 5 THEN 74.42
            END
        );

        INSERT INTO InvoiceLines (InvoiceID, ProductID, Quantity, UnitPrice)
        VALUES (@NewInvoiceID, @ProductID, @Quantity, @UnitPrice);
        
        SET @LineCounter = @LineCounter + 1;
    END

    SET @Counter = @Counter + 1;
END

PRINT 'Invoice generation complete.';
PRINT 'Updating invoice totals from line items...';

-- 5. Update Invoice Totals
-- This script runs *after* the loop to set all invoice 'Amount' fields
-- to the sum of their corresponding lines.
UPDATE I
SET I.Amount = ISNULL(L.TotalAmount, 0)
FROM Invoices I
LEFT JOIN (
    SELECT InvoiceID, SUM(Quantity * UnitPrice) AS TotalAmount
    FROM InvoiceLines
    GROUP BY InvoiceID
) L ON I.InvoiceID = L.InvoiceID
WHERE I.CustomerID = @CustID;

PRINT 'Invoice totals updated.';