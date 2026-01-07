using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation(CarMutation carMutation, SaleMutation saleMutation, OrderMutation orderMutation, MakeMutation makeMutation)
        {
            Name = "Mutation";

            var mutations = new ObjectGraphType[] { carMutation, saleMutation, orderMutation, makeMutation };

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
