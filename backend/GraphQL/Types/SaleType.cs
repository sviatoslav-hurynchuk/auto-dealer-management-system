using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class SaleType : ObjectGraphType<Sale>
    {
        public SaleType()
        {
            Field(x => x.Id);
            Field(x => x.CarId);
            Field(x => x.CustomerId);
            Field(x => x.EmployeeId);
            Field(x => x.SaleDate);
            Field(x => x.FinalPrice);
            Field(x => x.Status);
        }
    }
}
