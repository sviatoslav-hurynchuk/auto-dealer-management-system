using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class SaleMutation : ObjectGraphType
    {
        public SaleMutation(SaleService saleService)
        {
            Name = "SaleMutations";

            // ==============================
            // CREATE
            // ==============================
            Field<SaleType>("createSale")
                .Argument<NonNullGraphType<IntGraphType>>("carId")
                .Argument<NonNullGraphType<IntGraphType>>("customerId")
                .Argument<NonNullGraphType<IntGraphType>>("employeeId")
                .Argument<NonNullGraphType<DecimalGraphType>>("finalPrice")
                .Argument<StringGraphType>("status")
                .ResolveAsync(async context =>
                {
                    var sale = new Sale
                    {
                        CarId = context.GetArgument<int>("carId"),
                        CustomerId = context.GetArgument<int>("customerId"),
                        EmployeeId = context.GetArgument<int>("employeeId"),
                        FinalPrice = context.GetArgument<decimal>("finalPrice"),
                        Status = context.GetArgument<string?>("status") ?? "Completed",
                        SaleDate = DateTime.UtcNow
                    };

                    return await saleService.CreateSaleAsync(sale);
                });

            // ==============================
            // UPDATE
            // ==============================
            Field<SaleType>("updateSale")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .Argument<IntGraphType>("carId")
                .Argument<IntGraphType>("customerId")
                .Argument<IntGraphType>("employeeId")
                .Argument<DecimalGraphType>("finalPrice")
                .Argument<StringGraphType>("status")
                .ResolveAsync(async context =>
                {
                    var sale = await saleService.GetSaleByIdAsync(
                        context.GetArgument<int>("id")
                    );

                    sale.CarId = context.GetArgument<int?>("carId") ?? sale.CarId;
                    sale.CustomerId = context.GetArgument<int?>("customerId") ?? sale.CustomerId;
                    sale.EmployeeId = context.GetArgument<int?>("employeeId") ?? sale.EmployeeId;
                    sale.FinalPrice = context.GetArgument<decimal?>("finalPrice") ?? sale.FinalPrice;
                    sale.Status = context.GetArgument<string?>("status") ?? sale.Status;

                    return await saleService.UpdateSaleAsync(sale);
                });

            // ==============================
            // DELETE
            // ==============================
            Field<BooleanGraphType>("deleteSale")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await saleService.DeleteSaleAsync(id);
                    return true;
                });
        }
    }
}
