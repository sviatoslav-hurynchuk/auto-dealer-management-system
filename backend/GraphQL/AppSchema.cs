using backend.Exceptions;
using backend.GraphQL.Mutations;
using backend.GraphQL.Queries;
using GraphQL;
using GraphQL.Types;

namespace backend.GraphQL
{
    public class AppSchema : Schema
    {
        public AppSchema(IServiceProvider provider, ILogger<AppSchema> logger) : base(provider)
        {
            Query = provider.GetRequiredService<RootQuery>();
            Mutation = provider.GetRequiredService<RootMutation>();

            FieldMiddleware.Use(next => async context =>
            {
                try
                {
                    return await next(context);
                }
                catch (Exception ex)
                {
                    if (ex is not UnauthorizedException &&
                        ex is not ForbiddenException &&
                        ex is not ConflictException &&
                        ex is not NotFoundException &&
                        ex is not ValidationException)
                    {
                        logger.LogError(ex, "Unhandled exception in GraphQL field {Field}", context.FieldDefinition?.Name);
                    }

                    throw ex switch
                    {
                        UnauthorizedException => new ExecutionError(ex.Message, ex),
                        ForbiddenException => new ExecutionError(ex.Message, ex),
                        ConflictException => new ExecutionError(ex.Message, ex),
                        NotFoundException => new ExecutionError(ex.Message, ex),
                        ValidationException => new ExecutionError(ex.Message, ex),
                        _ => new ExecutionError(ex.Message, ex)
                    };
                }
            });
        }
    }
}
