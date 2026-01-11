using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class OrderTransactionMutation : ObjectGraphType
    {
        public OrderTransactionMutation(OrderTransactionService transactionService)
        {
            Name = "OrderTransactionMutations";

            // ==============================
            // CREATE ORDER WITH CAR
            // ==============================
            Field<OrderType>("createOrderWithNewCar")
                .Argument<NonNullGraphType<StringGraphType>>("makeName")

                // Car fields
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

                // Order fields
                .Argument<NonNullGraphType<StringGraphType>>("orderSupplierName")
                .Argument<NonNullGraphType<IntGraphType>>("quantity")
                .Argument<NonNullGraphType<DateGraphType>>("orderDate")
                .Argument<NonNullGraphType<StringGraphType>>("orderStatus")

                .ResolveAsync(async context =>
                {
                    // ==== Create Car object ====
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

                    // ==== Create Order object ====
                    var order = new Order
                    {
                        Quantity = context.GetArgument<int>("quantity"),
                        OrderDate = context.GetArgument<DateTime>("orderDate"),
                        Status = context.GetArgument<string>("orderStatus")
                    };

                    var supplierName = context.GetArgument<string>("orderSupplierName"); 

                    // ==== Call transactional service ====
                    return await transactionService.CreateOrderWithCarAsync(makeName, car, supplierName, order);
                });
        }
    }
}
