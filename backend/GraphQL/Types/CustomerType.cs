using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class CustomerType : ObjectGraphType<Customer>
    {
        public CustomerType()
        {
            Field(x => x.Id);
            Field(x => x.FullName);
            Field(x => x.Phone, nullable: true);
            Field(x => x.Email, nullable: true);
            Field(x => x.Address, nullable: true);
        }
    }
}
