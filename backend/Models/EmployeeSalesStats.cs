namespace backend.Models
{
    public class EmployeeSalesStats
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int SalesCount { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal AverageSaleAmount { get; set; }
        public DateTime? LastSaleDate { get; set; }
    }
}
