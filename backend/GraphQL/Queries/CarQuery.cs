using backend.GraphQL.Types;
using backend.Services;
using backend.Repositories.Interfaces;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class CarQuery : ObjectGraphType
    {
        public CarQuery(CarService carService, IMakeRepository makeRepository)
        {
            Name = "CarQueries";
            Field<ListGraphType<CarType>>("searchCars")
    .Argument<NonNullGraphType<CarSearchInputType>>("filter")
    .Argument<StringGraphType>("makeName")
    .ResolveAsync(async context =>
    {
        var filter = context.GetArgument<CarSearchParams>("filter");
        var makeName = context.GetArgument<string?>("makeName");
        if (!string.IsNullOrWhiteSpace(makeName))
        {
            var make = await makeRepository.GetMakeByNameAsync(makeName);
            if (make != null)
            {
                filter.MakeId = make.Id;
            }
            else
            {
                filter.MakeId = null;
            }
        }

        return await carService.SearchCarsAsync(filter);
    });

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

            Field<ListGraphType<CarWithSupplierAndSalesType>>("getCarsWithSupplierAndSales")
                .ResolveAsync(async context =>
                {
                    return await carService.GetCarsWithSupplierAndSalesAsync();
                });
        }
    }
}
