using backend.GraphQL.Queries;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery(CarQuery carQuery, SaleQuery saleQuery, OrderQuery orderQuery, MakeQuery makeQuery, SupplierQuery supplierQuery, EmployeeQuery employeeQuery)
        {
            Name = "Query";

            var queries = new ObjectGraphType[] { carQuery, saleQuery, orderQuery, makeQuery, supplierQuery, employeeQuery };

            foreach (var query in queries)
            {
                foreach (var field in query.Fields)
                {
                    AddField(field);
                }
            }
        }
    }
}
