using backend.GraphQL.Types;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class CustomerQuery : ObjectGraphType
    {
        public CustomerQuery(CustomerService customerService)
        {
            Name = "CustomerQueries";

            // ==============================
            // GET BY ID
            // ==============================
            Field<CustomerType>("getCustomerById")
                .Argument<NonNullGraphType<IntGraphType>>("customerId")
                .ResolveAsync(async context =>
                {
                    var customerId = context.GetArgument<int>("customerId");
                    return await customerService.GetCustomerByIdAsync(customerId);
                });

            // ==============================
            // GET ALL
            // ==============================
            Field<ListGraphType<CustomerType>>("getAllCustomers")
                .ResolveAsync(async _ =>
                {
                    return await customerService.GetAllCustomersAsync();
                });
        }
    }
}
