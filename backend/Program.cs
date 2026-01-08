using backend.GraphQL;
using backend.GraphQL.Mutations;
using backend.GraphQL.Queries;
using backend.GraphQL.Types;
using backend.Models;
using backend.Repositories;
using backend.Repositories.Interfaces;
using backend.Services;
using DotNetEnv;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Security.AccessControl;
using System.Text;

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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' is missing in appsettings.json.");
            }

            builder.Services.AddScoped<ICarRepository, CarRepository>(p => new CarRepository(connectionString!));
            builder.Services.AddScoped<IMakeRepository, MakeRepository>(p => new MakeRepository(connectionString!));
            //builder.Services.AddScoped<ICustomerRepository, CustomerRepository>(p => new CustomerRepository(connectionString!));
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>(p => new EmployeeRepository(connectionString!));
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>(p => new SupplierRepository(connectionString!));
            builder.Services.AddScoped<IOrderRepository, OrderRepository>(p => new OrderRepository(connectionString!));
            builder.Services.AddScoped<ISaleRepository, SaleRepository>(p => new SaleRepository(connectionString!));
           // builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>(p => new ServiceRequestRepository(connectionString!));


            builder.Services.AddScoped<CarService>();
            builder.Services.AddScoped<MakeService>();
            //builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<EmployeeService>();
            builder.Services.AddScoped<SupplierService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<SaleService>();
            //builder.Services.AddScoped<ServiceRequestService>();

          
            builder.Services.AddScoped<CarType>();
            builder.Services.AddScoped<MakeType>();
            //builder.Services.AddScoped<CustomerType>();
            builder.Services.AddScoped<EmployeeType>();
            builder.Services.AddScoped<SupplierType>();
            builder.Services.AddScoped<OrderType>();
            builder.Services.AddScoped<SaleType>();
            //builder.Services.AddScoped<ServiceRequestType>();

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
            // JWT
            // ==============================
            /*var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? throw new InvalidOperationException("JWT_SECRET_KEY is not set.");
            var expiryMinutesRaw = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINS")
                ?? throw new InvalidOperationException("JWT_EXPIRY_MINS is not set.");

            if (!int.TryParse(expiryMinutesRaw, out var expiryMinutes))
                throw new InvalidOperationException("JWT_EXPIRY_MINS must be int");

            builder.Services.AddSingleton(new JwtService(jwtKey!, expiryMinutes));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey!))
                };
            });
            */
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
