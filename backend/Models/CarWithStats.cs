public class CarWithStats
{
    public int Id { get; set; }
    public string Model { get; set; } = "";
    public string Vin { get; set; } = "";
    public decimal Price { get; set; }
    public string Status { get; set; } = "";
    public string SupplierName { get; set; } = "";
    public int OrdersCount { get; set; }
    public DateTime? LastSaleDate { get; set; }  // Nullable!
}
