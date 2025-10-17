using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(int id);
    }
}
