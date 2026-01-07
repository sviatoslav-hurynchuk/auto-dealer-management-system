using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class OrderType : ObjectGraphType<Order>
    {
        public OrderType()
        {
            Field(x => x.Id);
            Field(x => x.SupplierId);
            Field(x => x.CarId);
            Field(x => x.OrderDate);
            Field(x => x.Quantity);
            Field(x => x.Status);
        }
    }
}
