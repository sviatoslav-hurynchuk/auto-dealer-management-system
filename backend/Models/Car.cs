namespace backend.Models;

public class Car
{
    public int Id { get; set; }
    public int MakeId { get; set; }
    public string Model { get; set; } = null!;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public string? Color { get; set; }
    public string Vin { get; set; } = null!;
    public int? SupplierId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Condition { get; set; } // New / Used
    public int? Mileage { get; set; }
    public string? BodyType { get; set; }
    public string Status { get; set; } = null!;

}
