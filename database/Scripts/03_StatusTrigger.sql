CREATE TRIGGER trg_AfterSale
    ON Sales
    AFTER INSERT
AS
BEGIN
UPDATE Cars
SET Status = 'Sold'
WHERE id IN (SELECT CarID FROM inserted)
END
