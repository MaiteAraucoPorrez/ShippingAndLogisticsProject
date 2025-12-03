using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.Services;
using ShippingAndLogisticsManagement.Infrastructure.Data;
using ShippingAndLogisticsManagement.Infrastructure.Filters;
using ShippingAndLogisticsManagement.Infrastructure.Mappings;
using ShippingAndLogisticsManagement.Infrastructure.Repositories;
using ShippingAndLogisticsManagement.Infrastructure.Validator;
using ShippingAndLogisticsManagement.Infrastructure.Validators;

namespace ShippingAndLogisticsManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Configure Logging
            builder.Configuration.Sources.Clear();
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true, reloadOnChange: true);

            //Configure User Secrets for Development Environment
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
                Console.WriteLine("User Secrets habilitados para desarrollo");
            }


            #region BD SqlServer Configuration
            var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
            builder.Services.AddDbContext<LogisticContext>(options =>
                options.UseSqlServer(connectionString));
            #endregion

            #region AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            #endregion
           
            #region Dependency Injection - Services
            builder.Services.AddTransient<IShipmentService, ShipmentService>();
            builder.Services.AddTransient<IPackageService, PackageService>();
            builder.Services.AddTransient<ICustomerService, CustomerService>();
            builder.Services.AddTransient<IRouteService, RouteService>();
            builder.Services.AddTransient<IAddressService, AddressService>();
            builder.Services.AddTransient<IWarehouseService, WarehouseService>();
            builder.Services.AddTransient<IDriverService, DriverService>();
            builder.Services.AddTransient<IVehicleService,  VehicleService>();
            builder.Services.AddSingleton<IPasswordService, PasswordService>();
            #endregion

            #region Dependency Injection - Repositories & Infrastructure
            //builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
            //builder.Services.AddTransient<IRouteRepository, RouteRepository>();
            //builder.Services.AddTransient<IShipmentRepository, ShipmentRepository>();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();
            #endregion


            // Add services to the container
            #region Controllers Configuration
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            })
            // Avoid the automatic 400 response
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            #endregion


            #region Validations - FluentValidation
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidatorFilter>();
            });

            builder.Services.AddValidatorsFromAssemblyContaining<ShipmentDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<PackageDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CustomerDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<RouteDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<AddressDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<WarehouseDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ShipmentWarehouseDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<DriverDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<VehicleDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<GetByIdRequestValidator>();

            builder.Services.AddScoped<IValidatorService, ValidatorService>();
            builder.Services.AddScoped<ISecurityService, SecurityService>();
            //Password Options from Configuration
            builder.Services.Configure<Core.CustomEntities.PasswordOptions>
                (builder.Configuration.GetSection("PasswordOptions"));
            #endregion


            #region Swagger Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Backend Shipping And Logistics Management API",
                    Version = "v1",
                    Description = "Documentación de la API de Envíos y Logística - NET 9\n\n" +
                                  "Esta API gestiona el sistema completo de logística incluyendo:\n" +
                                  "- **Customers**: Gestión de clientes con validación de email único y límite de dominio\n" +
                                  "- **Routes**: Gestión de rutas con análisis de rentabilidad y ranking de uso\n" +
                                  "- **Shipments**: Gestión de envíos con validación de estados y límites por cliente\n" +
                                  "- **Packages**: Gestión de paquetes con validación de peso, precio y límites por envío",
                    Contact = new()
                    {
                        Name = "Equipo de Desarrollo UCB",
                        Email = "desarrollo@ucb.edu.bo"
                    }
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.EnableAnnotations();
            });
            #endregion


            #region API Versioning
            builder.Services.AddApiVersioning(options =>
            {
                // Reporta las versiones soportadas y obsoletas en encabezados de respuesta
                options.ReportApiVersions = true;

                // Versión por defecto si no se especifica
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Soporta versionado mediante URL, Header o QueryString
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),       // Ejemplo: /api/v1/...
                    new HeaderApiVersionReader("x-api-version"), // Ejemplo: Header → x-api-version: 1.0
                    new QueryStringApiVersionReader("api-version") // Ejemplo: ?api-version=1.0
                );
            });
            #endregion


            #region Authorization & Authentication - JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey
                    (
                        System.Text.Encoding.UTF8.GetBytes
                        (
                            builder.Configuration["Authentication:SecretKey"]
                        )
                    )
                };
            });
            #endregion


            //Environment Variables can override any configuration
            builder.Configuration.AddEnvironmentVariables();

            var app = builder.Build();

            #region Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Shipping And Logistics Management API v1");
                options.RoutePrefix = string.Empty;
                options.DocumentTitle = "Shipping & Logistics API Documentation";
                options.DefaultModelsExpandDepth(-1); // Hide schemas section
            });
            #endregion


            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

