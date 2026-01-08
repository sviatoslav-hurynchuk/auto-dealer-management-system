using backend.GraphQL.Types;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class MakeQuery : ObjectGraphType
    {
        public MakeQuery(MakeService makeService)
        {
            Name = "MakeQueries";

            // ==============================
            // GET ALL
            // ==============================
            Field<ListGraphType<MakeType>>("getAllMakes")
                .ResolveAsync(async _ =>
                    await makeService.GetAllMakesAsync());

            // ==============================
            // GET BY ID
            // ==============================
            Field<MakeType>("getMakeById")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    return await makeService.GetMakeByIdAsync(id);
                });
        }
    }
}
