using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class SupplierType : ObjectGraphType<Supplier>
    {
        public SupplierType()
        {
            Field(x => x.Id);
            Field(x => x.CompanyName);
            Field(x => x.ContactName, nullable: true);
            Field(x => x.Phone, nullable: true);
            Field(x => x.Email, nullable: true);
        }
    }
}
