using backend.GraphQL.Types;
using backend.Models;
using backend.Services;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL.Mutations
{
    public class ServiceRequestMutation : ObjectGraphType
    {
        public ServiceRequestMutation(ServiceRequestService serviceRequestService)
        {
            Name = "ServiceRequestMutations";

            // ==============================
            // CREATE
            // ==============================
            Field<ServiceRequestType>("createServiceRequest")
                .Argument<NonNullGraphType<IntGraphType>>("carId")
                .Argument<NonNullGraphType<StringGraphType>>("serviceType")
                .Argument<StringGraphType>("status")
                .ResolveAsync(async context =>
                {
                    var request = new ServiceRequest
                    {
                        CarId = context.GetArgument<int>("carId"),
                        ServiceType = context.GetArgument<string>("serviceType"),
                        Status = context.GetArgument<string?>("status") ?? "Pending"
                    };

                    return await serviceRequestService.CreateRequestAsync(request);
                });

            // ==============================
            // UPDATE
            // ==============================
            Field<ServiceRequestType>("updateServiceRequest")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .Argument<StringGraphType>("serviceType")
                .Argument<StringGraphType>("status")
                .ResolveAsync(async context =>
                {
                    var request = await serviceRequestService.GetRequestByIdAsync(
                        context.GetArgument<int>("id")
                    );

                    request.ServiceType =
                        context.GetArgument<string?>("serviceType") ?? request.ServiceType;

                    request.Status =
                        context.GetArgument<string?>("status") ?? request.Status;

                    return await serviceRequestService.UpdateRequestAsync(request);
                });

            // ==============================
            // DELETE
            // ==============================
            Field<BooleanGraphType>("deleteServiceRequest")
                .Argument<NonNullGraphType<IntGraphType>>("id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    await serviceRequestService.DeleteRequestAsync(id);
                    return true;
                });
        }
    }
}
