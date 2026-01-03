using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation()
        {
            Name = "Mutation";

            var mutations = new ObjectGraphType[] {};

            foreach (var mutation in mutations)
            {
                foreach (var field in mutation.Fields)
                {
                    AddField(field);
                }
            }
        }
    }
}
