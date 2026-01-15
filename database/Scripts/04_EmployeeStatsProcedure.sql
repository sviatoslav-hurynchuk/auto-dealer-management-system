CREATE PROCEDURE GetEmployeeSalesStats
    AS
BEGIN
SELECT
    e.Id AS EmployeeId,
    e.FullName AS EmployeeName,
    COUNT(s.Id) AS SalesCount,
    ISNULL(SUM(s.FinalPrice), 0) AS TotalSalesAmount,
    ISNULL(AVG(s.FinalPrice), 0) AS AverageSaleAmount,
    MAX(s.SaleDate) AS LastSaleDate
FROM Employees e
         LEFT JOIN Sales s ON s.EmployeeId = e.Id
GROUP BY
    e.Id, e.FullName
ORDER BY
    TotalSalesAmount DESC;
END
