using backend.GraphQL.Types;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class EmployeeQuery : ObjectGraphType
    {
        public EmployeeQuery(EmployeeService employeeService)
        {
            Name = "EmployeeQueries";

            // ==============================
            // GET BY ID
            // ==============================
            Field<EmployeeType>("getEmployeeById")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    return await employeeService.GetEmployeeByIdAsync(id);
                });

            // ==============================
            // GET ALL
            // ==============================
            Field<ListGraphType<EmployeeType>>("getAllEmployees")
                .Argument<BooleanGraphType>("activeOnly")
                .ResolveAsync(async context =>
                {
                    var activeOnly = context.GetArgument<bool?>("activeOnly") ?? false;
                    return await employeeService.GetAllEmployeesAsync(activeOnly);
                });
        }
    }
}
