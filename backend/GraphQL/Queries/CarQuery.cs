using backend.GraphQL.Types;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class CarQuery : ObjectGraphType
    {
        public CarQuery(CarService carService)
        {
            Name = "CarQueries";

            Field<CarType>("getCarById")
                .Argument<NonNullGraphType<IntGraphType>>("carId")
                .ResolveAsync(async context =>
                {
                    var carId = context.GetArgument<int>("carId");
                    return await carService.GetCarByIdAsync(carId);
                });

            Field<ListGraphType<CarType>>("getAllCars")
                .ResolveAsync(async context =>
                {
                    return await carService.GetAllCarsAsync();
                });
        }
    }
}
