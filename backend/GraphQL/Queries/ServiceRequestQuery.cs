using backend.GraphQL.Types;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Queries
{
    public class ServiceRequestQuery : ObjectGraphType
    {
        public ServiceRequestQuery(ServiceRequestService serviceRequestService)
        {
            Name = "ServiceRequestQueries";

            // ==============================
            // GET BY ID
            // ==============================
            Field<ServiceRequestType>("getServiceRequestById")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    return await serviceRequestService.GetRequestByIdAsync(id);
                });

            // ==============================
            // GET ALL
            // ==============================
            Field<ListGraphType<ServiceRequestType>>("getAllServiceRequests")
                .ResolveAsync(async _ =>
                {
                    return await serviceRequestService.GetAllRequestsAsync();
                });

            // ==============================
            // GET BY CAR
            // ==============================
            Field<ListGraphType<ServiceRequestType>>("getServiceRequestsByCarId")
                .Argument<NonNullGraphType<IntGraphType>>("carId")
                .ResolveAsync(async context =>
                {
                    var carId = context.GetArgument<int>("carId");
                    return await serviceRequestService.GetRequestsByCarIdAsync(carId);
                });
        }
    }
}
