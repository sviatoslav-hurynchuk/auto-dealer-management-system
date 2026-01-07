using backend.GraphQL.Types;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class SupplierQuery : ObjectGraphType
    {
        public SupplierQuery(SupplierService supplierService)
        {
            Name = "SupplierQueries";

            // ==============================
            // GET BY ID
            // ==============================
            Field<SupplierType>("getSupplierById")
                .Argument<NonNullGraphType<IntGraphType>>("supplierId")
                .ResolveAsync(async context =>
                {
                    var supplierId = context.GetArgument<int>("supplierId");
                    return await supplierService.GetSupplierByIdAsync(supplierId);
                });

            // ==============================
            // GET ALL
            // ==============================
            Field<ListGraphType<SupplierType>>("getAllSuppliers")
                .ResolveAsync(async context =>
                {
                    return await supplierService.GetAllSuppliersAsync();
                });
        }
    }
}
