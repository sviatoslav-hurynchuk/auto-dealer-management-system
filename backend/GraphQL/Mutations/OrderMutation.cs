using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class OrderMutation : ObjectGraphType
    {
        public OrderMutation(OrderService orderService)
        {
            Name = "OrderMutations";

            // ==============================
            // CREATE ORDER
            // ==============================
            Field<OrderType>("createOrderWithExistingCarAndSupplier")
                .Argument<NonNullGraphType<IntGraphType>>("supplierId")
                .Argument<NonNullGraphType<IntGraphType>>("carId")
                .Argument<NonNullGraphType<IntGraphType>>("quantity")
                .Argument<NonNullGraphType<StringGraphType>>("status")
                .ResolveAsync(async context =>
                {
                    var order = new Order
                    {
                        SupplierId = context.GetArgument<int>("supplierId"),
                        CarId = context.GetArgument<int>("carId"),
                        Quantity = context.GetArgument<int>("quantity"),
                        Status = context.GetArgument<string>("status"),
                        OrderDate = DateTime.UtcNow
                    };

                    return await orderService.CreateOrderAsync(order);
                });

            // ==============================
            // UPDATE ORDER
            // ==============================
            Field<OrderType>("updateOrder")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .Argument<IntGraphType>("supplierId")
                .Argument<IntGraphType>("carId")
                .Argument<IntGraphType>("quantity")
                .Argument<StringGraphType>("status")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    var existing = await orderService.GetOrderByIdAsync(id);

                    existing.SupplierId = context.GetArgument<int?>("supplierId") ?? existing.SupplierId;
                    existing.CarId = context.GetArgument<int?>("carId") ?? existing.CarId;
                    existing.Quantity = context.GetArgument<int?>("quantity") ?? existing.Quantity;
                    existing.Status = context.GetArgument<string?>("status") ?? existing.Status;

                    return await orderService.UpdateOrderAsync(existing);
                });

            // ==============================
            // DELETE ORDER
            // ==============================
            Field<BooleanGraphType>("deleteOrder")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await orderService.DeleteOrderAsync(id);
                    return true;
                });
        }
    }
}
