using backend.Models;
using GraphQL.Types;

namespace backend.GraphQL.Types
{
    public class ServiceRequestType : ObjectGraphType<ServiceRequest>
    {
        public ServiceRequestType()
        {
            Field(x => x.Id);
            Field(x => x.CarId);
            Field(x => x.ServiceType);
            Field(x => x.Status);
            Field(x => x.UpdatedAt);
        }
    }
}
