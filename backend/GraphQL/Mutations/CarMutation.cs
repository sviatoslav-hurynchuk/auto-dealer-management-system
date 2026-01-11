using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class CarMutation : ObjectGraphType
    {
        /// <summary>
        /// Registers GraphQL mutation fields for creating, updating, and deleting car entities.
        /// </summary>
        /// <remarks>
        /// - createCar: accepts a make name and car properties, creates a new Car and delegates creation to the service handling make association.
        /// - updateCar: loads an existing Car by id, applies any provided fields, and updates the Car via the service.
        /// - deleteCar: deletes a Car by id and returns a boolean confirmation.
        /// </remarks>
        public CarMutation(CarService carService)
        {
            Name = "CarMutations";

            // ==============================
            // CREATE
            // ==============================
            Field<CarType>("createCar")
    .Argument<NonNullGraphType<StringGraphType>>("makeName")
                .Argument<NonNullGraphType<StringGraphType>>("model")
                .Argument<NonNullGraphType<IntGraphType>>("year")
                .Argument<NonNullGraphType<DecimalGraphType>>("price")
                .Argument<StringGraphType>("color")
                .Argument<NonNullGraphType<StringGraphType>>("vin")
                .Argument<IntGraphType>("supplierId")
                .Argument<StringGraphType>("description")
                .Argument<StringGraphType>("imageUrl")
                .Argument<StringGraphType>("condition")
                .Argument<IntGraphType>("mileage")
                .Argument<StringGraphType>("bodyType")
                .Argument<NonNullGraphType<StringGraphType>>("status")
                .ResolveAsync(async context =>
                {
                    var car = new Car
                    {
                        Model = context.GetArgument<string>("model"),
                        Year = context.GetArgument<int>("year"),
                        Price = context.GetArgument<decimal>("price"),
                        Color = context.GetArgument<string?>("color"),
                        Vin = context.GetArgument<string>("vin"),
                        SupplierId = context.GetArgument<int?>("supplierId"),
                        Description = context.GetArgument<string?>("description"),
                        ImageUrl = context.GetArgument<string?>("imageUrl"),
                        Condition = context.GetArgument<string?>("condition"),
                        Mileage = context.GetArgument<int?>("mileage"),
                        BodyType = context.GetArgument<string?>("bodyType"),
                        Status = context.GetArgument<string>("status")
                    };

                    var makeName = context.GetArgument<string>("makeName");

                    return await carService.CreateCarWithMakeAsync(makeName, car);
                });

            // ==============================
            // UPDATE
            // ==============================
            Field<CarType>("updateCar")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .Argument<IntGraphType>("makeId")
                .Argument<StringGraphType>("model")
                .Argument<IntGraphType>("year")
                .Argument<DecimalGraphType>("price")
                .Argument<StringGraphType>("color")
                .Argument<StringGraphType>("vin")
                .Argument<IntGraphType>("supplierId")
                .Argument<StringGraphType>("description")
                .Argument<StringGraphType>("imageUrl")
                .Argument<StringGraphType>("condition")
                .Argument<IntGraphType>("mileage")
                .Argument<StringGraphType>("bodyType")
                .Argument<StringGraphType>("status")
                .ResolveAsync(async context =>
                {
                    var car = await carService.GetCarByIdAsync(context.GetArgument<int>("id"));

                    car.MakeId = context.GetArgument<int?>("makeId") ?? car.MakeId;
                    car.Model = context.GetArgument<string?>("model") ?? car.Model;
                    car.Year = context.GetArgument<int?>("year") ?? car.Year;
                    car.Price = context.GetArgument<decimal?>("price") ?? car.Price;
                    car.Color = context.GetArgument<string?>("color") ?? car.Color;
                    car.Vin = context.GetArgument<string?>("vin") ?? car.Vin;
                    car.SupplierId = context.GetArgument<int?>("supplierId") ?? car.SupplierId;
                    car.Description = context.GetArgument<string?>("description") ?? car.Description;
                    car.ImageUrl = context.GetArgument<string?>("imageUrl") ?? car.ImageUrl;
                    car.Condition = context.GetArgument<string?>("condition") ?? car.Condition;
                    car.Mileage = context.GetArgument<int?>("mileage") ?? car.Mileage;
                    car.BodyType = context.GetArgument<string?>("bodyType") ?? car.BodyType;
                    car.Status = context.GetArgument<string?>("status") ?? car.Status;

                    return await carService.UpdateCarAsync(car);
                });

            // ==============================
            // DELETE
            // ==============================
            Field<BooleanGraphType>("deleteCar")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await carService.DeleteCarAsync(id);
                    return true;
                });
        }
    }
}