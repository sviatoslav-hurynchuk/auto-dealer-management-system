using GraphQL.Types;

public class CarSearchInputType : InputObjectGraphType
{
    public CarSearchInputType()
    {
        Name = "CarSearchInput";

        Field<IntGraphType>("makeId");
        Field<StringGraphType>("makeName");
        Field<StringGraphType>("model");
        Field<StringGraphType>("vin");
        Field<StringGraphType>("color");
        Field<StringGraphType>("condition");
        Field<StringGraphType>("bodyType");
        Field<StringGraphType>("status");

        Field<IntGraphType>("yearFrom");
        Field<IntGraphType>("yearTo");

        Field<DecimalGraphType>("priceFrom");
        Field<DecimalGraphType>("priceTo");

        Field<StringGraphType>("sortBy");
        Field<StringGraphType>("sortDirection");

        Field<IntGraphType>("page");
        Field<IntGraphType>("pageSize");
    }
}
