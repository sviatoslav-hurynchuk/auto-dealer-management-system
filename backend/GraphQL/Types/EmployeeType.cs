using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class EmployeeType : ObjectGraphType<Employee>
    {
        public EmployeeType()
        {
            Name = "Employee";

            Field(x => x.Id);
            Field(x => x.FullName);
            Field(x => x.Position, nullable: true);
            Field(x => x.Phone, nullable: true);
            Field(x => x.Email, nullable: true);
            Field(x => x.IsActive);

        }
    }
}
