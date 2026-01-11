using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class CarInputType : InputObjectGraphType<Car>
    {
        public CarInputType()
        {
            Name = "CarInput";
            Field(x => x.Model);
            Field(x => x.Year);
            Field(x => x.Price);
            Field(x => x.Color, nullable: true);
            Field(x => x.Vin);
            Field(x => x.SupplierId, nullable: true);
            Field(x => x.Description, nullable: true);
            Field(x => x.ImageUrl, nullable: true);
            Field(x => x.Condition, nullable: true);
            Field(x => x.Mileage, nullable: true);
            Field(x => x.BodyType, nullable: true);
            Field(x => x.Status);
        }
    }
}
