using backend.GraphQL.Types;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class OrderQuery : ObjectGraphType
    {
        public OrderQuery(OrderService orderService)
        {
            Name = "OrderQueries";

            // ==============================
            // GET ORDER BY ID
            // ==============================
            Field<OrderType>("getOrderById")
                .Argument<NonNullGraphType<IntGraphType>>("orderId")
                .ResolveAsync(async context =>
                {
                    var orderId = context.GetArgument<int>("orderId");
                    return await orderService.GetOrderByIdAsync(orderId);
                });

            // ==============================
            // GET ALL ORDERS
            // ==============================
            Field<ListGraphType<OrderType>>("getAllOrders")
                .ResolveAsync(async context =>
                {
                    return await orderService.GetAllOrdersAsync();
                });
        }
    }
}
