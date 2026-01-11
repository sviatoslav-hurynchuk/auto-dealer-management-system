namespace backend.Models;

public class Order
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public int CarId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = null!;
}
