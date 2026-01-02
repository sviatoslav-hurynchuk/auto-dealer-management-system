namespace backend.Models;

public class Supplier
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
