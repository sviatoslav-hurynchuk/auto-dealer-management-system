using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class CarType : ObjectGraphType<Car>
    {
        public CarType()
        {
            Field(x => x.Id);
            Field(x => x.MakeId);
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
