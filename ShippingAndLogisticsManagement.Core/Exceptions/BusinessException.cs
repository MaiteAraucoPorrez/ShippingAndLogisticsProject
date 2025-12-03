namespace ShippingAndLogisticsManagement.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur during business logic execution.
    /// </summary>
    /// <remarks>Use this exception to indicate that a business rule or validation has been violated. This
    /// exception is intended for scenarios where an operation fails due to domain-specific constraints, rather than
    /// system or infrastructure errors.</remarks>
    public class BusinessException: Exception
    {
        public BusinessException()
        {
        }
        public BusinessException(string? message) : base(message)
        {

        }
    }
}
