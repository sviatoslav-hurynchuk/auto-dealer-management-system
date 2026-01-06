using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation(CarMutation carMutation, SaleMutation saleMutation)
        {
            Name = "Mutation";

            var mutations = new ObjectGraphType[] { carMutation, saleMutation };

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
