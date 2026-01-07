using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class MakeMutation : ObjectGraphType
    {
        public MakeMutation(MakeService makeService)
        {
            Name = "MakeMutations";

            // ==============================
            // CREATE
            // ==============================
            Field<MakeType>("createMake")
                .Argument<NonNullGraphType<StringGraphType>>("name")
                .ResolveAsync(async context =>
                {
                    var make = new Make
                    {
                        Name = context.GetArgument<string>("name")
                    };

                    return await makeService.CreateMakeAsync(make);
                });

            // ==============================
            // UPDATE
            // ==============================
            Field<MakeType>("updateMake")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .Argument<NonNullGraphType<StringGraphType>>("name")
                .ResolveAsync(async context =>
                {
                    var make = new Make
                    {
                        Id = context.GetArgument<int>("id"),
                        Name = context.GetArgument<string>("name")
                    };

                    return await makeService.UpdateMakeAsync(make);
                });

            // ==============================
            // DELETE
            // ==============================
            Field<BooleanGraphType>("deleteMake")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await makeService.DeleteMakeAsync(id);
                    return true;
                });
        }
    }
}
