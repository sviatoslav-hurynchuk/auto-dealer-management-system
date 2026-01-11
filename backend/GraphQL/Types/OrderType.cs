using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class OrderType : ObjectGraphType<Order>
    {
        /// <summary>
        /// Configures the GraphQL fields exposed for the Order GraphQL type.
        /// </summary>
        /// <remarks>
        /// Exposes the following Order properties as GraphQL fields: Id, SupplierId, CarId, OrderDate, and Status.
        /// </remarks>
        public OrderType()
        {
            Field(x => x.Id);
            Field(x => x.SupplierId);
            Field(x => x.CarId);
            Field(x => x.OrderDate);
            Field(x => x.Status);
        }
    }
}