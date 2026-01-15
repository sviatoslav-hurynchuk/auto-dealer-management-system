using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class CarWithSupplierAndSalesType : ObjectGraphType<dynamic>
    {
        public CarWithSupplierAndSalesType()
        {
            Name = "CarWithSupplierAndSales";

            Field<IntGraphType>("id");
            Field<StringGraphType>("model");
            Field<StringGraphType>("vin");
            Field<DecimalGraphType>("price");
            Field<StringGraphType>("status");
            Field<StringGraphType>("supplierName");
            Field<IntGraphType>("ordersCount");
            Field<DateTimeGraphType>("lastSaleDate");
        }
    }
}
