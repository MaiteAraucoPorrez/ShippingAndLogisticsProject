using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.Data;

namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class CustomerRepository: ICustomerRepository
    {
        private readonly LogisticContext _context;
        public CustomerRepository(LogisticContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (customer == null)
                throw new InvalidOperationException($"No se encontró el cliente con Id {id}");
            return customer;
        }
    }
}
