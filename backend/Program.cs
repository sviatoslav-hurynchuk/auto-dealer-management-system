using backend.GraphQL;
using backend.GraphQL.Mutations;
using backend.GraphQL.Queries;
using backend.GraphQL.Types;
using backend.Repositories;
using backend.Repositories.Interfaces;
using backend.Services;
using DotNetEnv;
using GraphQL;
using GraphQL.Types;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);
            var env = builder.Environment;

            builder.Logging.AddConsole();

            builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            builder.Services.AddScoped<ICarRepository, CarRepository>();
            builder.Services.AddScoped<IMakeRepository, MakeRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ISaleRepository, SaleRepository>();
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();


            builder.Services.AddScoped<CarService>();
            builder.Services.AddScoped<MakeService>();
            builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<EmployeeService>();
            builder.Services.AddScoped<SupplierService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<SaleService>();
            builder.Services.AddScoped<ServiceRequestService>();

            builder.Services.AddScoped<CarType>();
            builder.Services.AddScoped<MakeType>();
            builder.Services.AddScoped<CustomerType>();
            builder.Services.AddScoped<EmployeeType>();
            builder.Services.AddScoped<SupplierType>();
            builder.Services.AddScoped<OrderType>();
            builder.Services.AddScoped<SaleType>();
            builder.Services.AddScoped<ServiceRequestType>();

            builder.Services.AddScoped<RootQuery>();
            builder.Services.AddScoped<RootMutation>();
            builder.Services.AddScoped<ISchema, AppSchema>();

            builder.Services.AddGraphQL(options =>
            {
                options.AddSystemTextJson();
                options.AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = false);
                options.AddGraphTypes(typeof(RootQuery).Assembly);
            });

            // ==============================
            // CORS
            // ==============================
            var frontendOrigin = Environment.GetEnvironmentVariable("FRONTEND_URL")
                ?? throw new InvalidOperationException("FRONTEND_URL is not set");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy
                        .WithOrigins(frontendOrigin)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            // ==============================
            // Controllers & HTTP
            // ==============================
            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseGraphQLGraphiQL("/ui/graphiql");
            }

            app.UseCors("AllowAll");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseGraphQL<ISchema>("/graphql");

            app.MapControllers();
            app.Run();
        }
    }
}
