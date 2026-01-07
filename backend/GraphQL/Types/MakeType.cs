using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class MakeType : ObjectGraphType<Make>
    {
        public MakeType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
        }
    }
}
