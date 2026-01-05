using backend.GraphQL.Types;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class SaleQuery : ObjectGraphType
    {
        public SaleQuery(SaleService saleService)
        {
            Name = "SaleQueries";

            // ==============================
            // GET BY ID
            // ==============================
            Field<SaleType>("getSaleById")
                .Argument<NonNullGraphType<IntGraphType>>("saleId")
                .ResolveAsync(async context =>
                {
                    var saleId = context.GetArgument<int>("saleId");
                    return await saleService.GetSaleByIdAsync(saleId);
                });

            // ==============================
            // GET ALL
            // ==============================
            Field<ListGraphType<SaleType>>("getAllSales")
                .ResolveAsync(async context =>
                {
                    return await saleService.GetAllSalesAsync();
                });
        }
    }
}
