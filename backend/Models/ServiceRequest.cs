namespace backend.Models;

public class ServiceRequest
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public string ServiceType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? UpdatedAt { get; set; }
}