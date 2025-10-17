using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ShippingAndLogisticsManagement.Infrastructure.Validator
{
    public interface IValidatorService
    {
        Task<ValidationResult> ValidateAsync<T>(T model);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
    public class ValidatorService : IValidatorService
    { 
        private readonly IServiceProvider _serviceProvider;
        public ValidatorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<ValidationResult> ValidateAsync<T>(T model)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>();
            if (validator == null)
            {
                throw new InvalidOperationException($"Validator not found for type {typeof(T).Name}");
            }
            var result = await validator.ValidateAsync(model);
            return new ValidationResult
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
            };
        }
    }
}
