using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class EmployeeMutation : ObjectGraphType
    {
        public EmployeeMutation(EmployeeService employeeService)
        {
            Name = "EmployeeMutations";

            // ==============================
            // CREATE
            // ==============================
            Field<EmployeeType>("createEmployee")
                .Argument<NonNullGraphType<StringGraphType>>("fullName")
                .Argument<StringGraphType>("position")
                .Argument<StringGraphType>("phone")
                .Argument<StringGraphType>("email")
                .ResolveAsync(async context =>
                {
                    var employee = new Employee
                    {
                        FullName = context.GetArgument<string>("fullName"),
                        Position = context.GetArgument<string?>("position"),
                        Phone = context.GetArgument<string?>("phone"),
                        Email = context.GetArgument<string?>("email")
                    };

                    return await employeeService.CreateEmployeeAsync(employee);
                });

            // ==============================
            // UPDATE (NO isActive!)
            // ==============================
            Field<EmployeeType>("updateEmployee")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .Argument<StringGraphType>("fullName")
                .Argument<StringGraphType>("position")
                .Argument<StringGraphType>("phone")
                .Argument<StringGraphType>("email")
                .ResolveAsync(async context =>
                {
                    var employee = await employeeService.GetEmployeeByIdAsync(
                        context.GetArgument<int>("id"));

                    if (context.HasArgument("fullName"))
                        employee.FullName = context.GetArgument<string>("fullName");

                    if (context.HasArgument("position"))
                        employee.Position = context.GetArgument<string?>("position");

                    if (context.HasArgument("phone"))
                        employee.Phone = context.GetArgument<string?>("phone");

                    if (context.HasArgument("email"))
                        employee.Email = context.GetArgument<string?>("email");

                    return await employeeService.UpdateEmployeeAsync(employee);
                });

            // ==============================
            // DEACTIVATE (SOFT DELETE)
            // ==============================
            Field<BooleanGraphType>("deactivateEmployee")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await employeeService.DeactivateEmployeeAsync(id);
                    return true;
                });

            // ==============================
            // DELETE (HARD)
            // ==============================
            Field<BooleanGraphType>("deleteEmployee")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await employeeService.DeleteEmployeeAsync(id);
                    return true;
                });
        }
    }
}
