using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
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

            //Configure User Secrets for Development Environment
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
            }
            //In Production the secrets will come from Global Environments


            #region BD SqlServer Configuration
            var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
            builder.Services.AddDbContext<LogisticContext>(options =>
                options.UseSqlServer(connectionString));
            #endregion

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Dependency injection
            //builder.Services.AddTransient<IShipmentRepository, ShipmentRepository>();
            builder.Services.AddTransient<IShipmentService, ShipmentService>();
            //builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
            //builder.Services.AddTransient<IRouteRepository, RouteRepository>();
            //builder.Services.AddTransient<IValidator<ShipmentDto>, ShipmentDtoValidator>();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();


            // Add services to the container
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })

            // Avoid the automatic 400 response
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Validations
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidatorFilter>();
            });

            //Configure Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Backend Shipping And Logistics Management API",
                    Version = "v1",
                    Description = "Documentacion de la API de Envios y Logistica - NET 9",
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

            // Fluent Validation
            builder.Services.AddValidatorsFromAssemblyContaining<ShipmentDtoValidator>();
                builder.Services.AddValidatorsFromAssemblyContaining<GetByIdRequestValidator>();

                // Services
                builder.Services.AddScoped<IValidatorService, ValidatorService>();

                var app = builder.Build();

                // Using Swagger
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Shipping And Logistics Management API v1");
                        options.RoutePrefix = string.Empty;
                    });
                }

                // Configure the HTTP request pipeline.

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
        }
    }
}

