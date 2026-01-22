public class CarSearchParams
{
    // filters
    public int? MakeId { get; set; }
    public string? Model { get; set; }
    public string? MakeName { get; set; }
    public string? Vin { get; set; }
    public string? Color { get; set; }
    public string? Condition { get; set; }
    public string? BodyType { get; set; }
    public string? Status { get; set; }

    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }

    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }

    // sorting
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }

    // pagination
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
