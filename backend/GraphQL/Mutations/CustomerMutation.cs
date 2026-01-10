using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class CustomerMutation : ObjectGraphType
    {
        public CustomerMutation(CustomerService customerService)
        {
            Name = "CustomerMutations";

            // ==============================
            // CREATE
            // ==============================
            Field<CustomerType>("createCustomer")
                .Argument<NonNullGraphType<StringGraphType>>("fullName")
                .Argument<StringGraphType>("phone")
                .Argument<StringGraphType>("email")
                .Argument<StringGraphType>("address")
                .ResolveAsync(async context =>
                {
                    var customer = new Customer
                    {
                        FullName = context.GetArgument<string>("fullName"),
                        Phone = context.GetArgument<string?>("phone"),
                        Email = context.GetArgument<string?>("email"),
                        Address = context.GetArgument<string?>("address")
                    };

                    return await customerService.CreateCustomerAsync(customer);
                });

            // ==============================
            // UPDATE
            // ==============================
            Field<CustomerType>("updateCustomer")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .Argument<StringGraphType>("fullName")
                .Argument<StringGraphType>("phone")
                .Argument<StringGraphType>("email")
                .Argument<StringGraphType>("address")
                .ResolveAsync(async context =>
                {
                    var customer = await customerService.GetCustomerByIdAsync(
                        context.GetArgument<int>("id")
                    );

                    customer.FullName =
                        context.GetArgument<string?>("fullName") ?? customer.FullName;
                    customer.Phone =
                        context.GetArgument<string?>("phone") ?? customer.Phone;
                    customer.Email =
                        context.GetArgument<string?>("email") ?? customer.Email;
                    customer.Address =
                        context.GetArgument<string?>("address") ?? customer.Address;

                    return await customerService.UpdateCustomerAsync(customer);
                });

            // DELETE
            // ==============================
            Field<BooleanGraphType>("deleteCustomer")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await customerService.DeleteCustomerAsync(id);
                    return true;
                });
        }
    }
}
