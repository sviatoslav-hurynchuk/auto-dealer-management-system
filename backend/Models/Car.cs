namespace backend.Models
{
    public class Car
    {
                public int Id { get; set; }

                public int MakeID { get; set; }

                public string Model { get; set; } = string.Empty;

                public int Year { get; set; }

                public decimal Price { get; set; }

                        public string? Color { get; set; }

                public string VIN { get; set; } = string.Empty;

                        public int? SupplierID { get; set; }

                public string Status { get; set; } = "In stock";
    }
}