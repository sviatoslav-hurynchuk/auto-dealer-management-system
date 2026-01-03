namespace backend.Models;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
