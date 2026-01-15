using backend.Models;
using GraphQL.Types;

public class EmployeeSalesStatsType : ObjectGraphType<EmployeeSalesStats>
{
    public EmployeeSalesStatsType()
    {
        Field(x => x.EmployeeId);
        Field(x => x.EmployeeName);
        Field(x => x.SalesCount);
        Field(x => x.TotalSalesAmount);
        Field(x => x.AverageSaleAmount);
        Field(x => x.LastSaleDate);
    }
}
