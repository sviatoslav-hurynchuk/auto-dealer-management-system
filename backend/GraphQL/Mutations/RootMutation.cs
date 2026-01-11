using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class RootMutation : ObjectGraphType
    {
        /// <summary>
        /// Constructs the root GraphQL mutation by aggregating fields from the provided sub-mutation objects.
        /// </summary>
        public RootMutation(CarMutation carMutation, 
            SaleMutation saleMutation, 
            OrderMutation orderMutation, 
            MakeMutation makeMutation, 
            SupplierMutation supplierMutation, 
            EmployeeMutation employeeMutation,
            CustomerMutation customerMutation,
            ServiceRequestMutation serviceRequestMutation
            )
        {
            Name = "Mutation";

            var mutations = new ObjectGraphType[] { carMutation, 
                saleMutation, 
                orderMutation, 
                makeMutation, 
                supplierMutation, 
                employeeMutation, 
                customerMutation,
            serviceRequestMutation
            };

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