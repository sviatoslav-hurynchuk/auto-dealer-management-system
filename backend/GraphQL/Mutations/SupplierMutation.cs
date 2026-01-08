using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class SupplierMutation : ObjectGraphType
    {
        public SupplierMutation(SupplierService supplierService)
        {
            Name = "SupplierMutations";

            // ==============================
            // CREATE
            // ==============================
            Field<SupplierType>("createSupplier")
                .Argument<NonNullGraphType<StringGraphType>>("companyName")
                .Argument<StringGraphType>("contactName")
                .Argument<StringGraphType>("phone")
                .Argument<StringGraphType>("email")
                .ResolveAsync(async context =>
                {
                    var supplier = new Supplier
                    {
                        CompanyName = context.GetArgument<string>("companyName"),
                        ContactName = context.GetArgument<string?>("contactName"),
                        Phone = context.GetArgument<string?>("phone"),
                        Email = context.GetArgument<string?>("email")
                    };

                    return await supplierService.CreateSupplierAsync(supplier);
                });

            // ==============================
            // UPDATE
            // ==============================
            Field<SupplierType>("updateSupplier")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .Argument<StringGraphType>("companyName")
                .Argument<StringGraphType>("contactName")
                .Argument<StringGraphType>("phone")
                .Argument<StringGraphType>("email")
                .ResolveAsync(async context =>
                {
                    var supplier = await supplierService.GetSupplierByIdAsync(
                        context.GetArgument<int>("id")
                    );

                    supplier.CompanyName =
                        context.GetArgument<string?>("companyName") ?? supplier.CompanyName;
                    supplier.ContactName =
                        context.GetArgument<string?>("contactName") ?? supplier.ContactName;
                    supplier.Phone =
                        context.GetArgument<string?>("phone") ?? supplier.Phone;
                    supplier.Email =
                        context.GetArgument<string?>("email") ?? supplier.Email;

                    return await supplierService.UpdateSupplierAsync(supplier);
                });

            // ==============================
            // DELETE
            // ==============================
            Field<BooleanGraphType>("deleteSupplier")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await supplierService.DeleteSupplierAsync(id);
                    return true;
                });
        }
    }
}
