using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class OrderInputType : InputObjectGraphType<Order>
    {
        public OrderInputType()
        {
            Name = "OrderInput";
            Field(x => x.SupplierId);
            Field(x => x.Quantity);
            Field(x => x.OrderDate);
            Field(x => x.Status);
        }
    }
}
