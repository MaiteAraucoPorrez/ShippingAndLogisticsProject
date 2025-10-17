using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.Services;
using ShippingAndLogisticsManagement.Infrastructure.Data;
using ShippingAndLogisticsManagement.Infrastructure.DTOS;
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

            #region BD SqlServer Configuration
            var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
            builder.Services.AddDbContext<LogisticContext>(options =>
                options.UseSqlServer(connectionString));
            #endregion

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Dependency injection
            builder.Services.AddTransient<IShipmentRepository, ShipmentRepository>();
            builder.Services.AddTransient<IShipmentService, ShipmentService>();
            builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
            builder.Services.AddTransient<IRouteRepository, RouteRepository>();

            // Add services to the container
            builder.Services.AddTransient<IValidator<ShipmentDto>, ShipmentDtoValidator>();
            builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();

            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add<ValidatorFilter>();
            })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })

            //Avoid the automatic 400 response
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //Validations
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidatorFilter>();
            });

            //Fluent Validation
            
            builder.Services.AddValidatorsFromAssemblyContaining<ShipmentDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<GetByIdRequestValidator>();

            //Inyección del servicio de validación Services
            builder.Services.AddScoped<IValidatorService, ValidatorService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
