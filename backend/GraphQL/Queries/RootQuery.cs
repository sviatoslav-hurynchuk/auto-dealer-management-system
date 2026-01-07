using backend.GraphQL.Queries;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery(CarQuery carQuery, SaleQuery saleQuery, OrderQuery orderQuery, MakeQuery makeQuery)
        {
            Name = "Query";

            var queries = new ObjectGraphType[] { carQuery, saleQuery, orderQuery, makeQuery };

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
