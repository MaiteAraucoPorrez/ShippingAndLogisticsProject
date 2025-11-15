using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(CustomerQueryFilter customerQueryFilter)
        {
            if (customerQueryFilter == null)
                throw new ArgumentNullException(nameof(customerQueryFilter));

            // Basic validations
            if (customerQueryFilter.PageNumber <= 0) customerQueryFilter.PageNumber = 1;
            if (customerQueryFilter.PageSize <= 0) customerQueryFilter.PageSize = 10;

            var customers = await _unitOfWork.CustomerRepository.GetAllDapperAsync(customerQueryFilter);

            // Applying filters
            if (!string.IsNullOrWhiteSpace(customerQueryFilter.Name))
                customers = customers.Where(c => c.Name.Contains(customerQueryFilter.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(customerQueryFilter.Email))
                customers = customers.Where(c => c.Email.Contains(customerQueryFilter.Email, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(customerQueryFilter.Phone))
                customers = customers.Where(c => c.Phone.Contains(customerQueryFilter.Phone));

            if (customerQueryFilter.HasActiveShipments.HasValue)
            {
                var customersWithStatus = new List<Customer>();
                foreach (var customer in customers)
                {
                    var hasActive = await _unitOfWork.CustomerRepository.HasActiveShipmentsAsync(customer.Id);
                    if (hasActive == customerQueryFilter.HasActiveShipments.Value)
                        customersWithStatus.Add(customer);
                }
                customers = customersWithStatus;
            }

            // Creating PagedList
            var pageNumber = customerQueryFilter.PageNumber;
            var pageSize = customerQueryFilter.PageSize;
            var sourceList = customers.ToList();
            var count = sourceList.Count;
            var items = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Building PagedList<object> for ResponseData
            var itemsAsObject = items.Cast<object>().ToList();
            var paged = new PagedList<object>(itemsAsObject, count, pageNumber, pageSize);

            var messages = new Message[] { new() { Type = "Information", Description = "Clientes recuperados correctamente" } };

            return new ResponseData()
            {
                Messages = messages,
                Pagination = paged,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<IEnumerable<Customer>> GetAllDapperAsync()
        {
            var customers = await _unitOfWork.CustomerRepository.GetRecentCustomersAsync(10);
            return customers;
        }

        public async Task<IEnumerable<CustomerShipmentHistoryResponse>> GetCustomerShipmentHistoryAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("El ID del cliente debe ser mayor a 0", nameof(customerId));

            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"El cliente con ID {customerId} no existe");

            var history = await _unitOfWork.CustomerRepository.GetCustomerShipmentHistoryAsync(customerId);

            if (history == null || !history.Any())
                throw new KeyNotFoundException($"El cliente no tiene envíos registrados");

            return history;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del cliente debe ser mayor a 0");

            var customer = await _unitOfWork.CustomerRepository.GetById(id);
            return customer;
        }

        public async Task<Customer> GetByIdDapperAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del cliente debe ser mayor a 0");

            var customer = await _unitOfWork.CustomerRepository.GetByIdDapperAsync(id);
            return customer;
        }

        public async Task InsertAsync(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));

            // Business validations
            if (string.IsNullOrWhiteSpace(customer.Name) || customer.Name.Length < 3 || customer.Name.Length > 100)
                throw new BusinessException("El nombre debe tener entre 3 y 100 caracteres");

            // Email validation
            if (string.IsNullOrWhiteSpace(customer.Email) || !IsValidEmail(customer.Email))
                throw new BusinessException("El email no tiene un formato válido");

            // Check unique email
            var existingEmail = await _unitOfWork.CustomerRepository.GetByEmailAsync(customer.Email);
            if (existingEmail != null)
                throw new BusinessException("Ya existe un cliente con ese email");

            // Check email domain limit (max 5 customers per domain)
            var emailDomain = customer.Email.Split('@').Last();
            var domainCount = await _unitOfWork.CustomerRepository.CountByEmailDomainAsync(emailDomain);
            if (domainCount >= 5)
                throw new BusinessException($"Se alcanzó el límite de 5 clientes con el dominio @{emailDomain}");

            // Phone validation
            if (string.IsNullOrWhiteSpace(customer.Phone) || customer.Phone.Length < 7 || customer.Phone.Length > 20)
                throw new BusinessException("El teléfono debe tener entre 7 y 20 caracteres");

            if (!Regex.IsMatch(customer.Phone, @"^[\d\s\-\+\(\)]+$"))
                throw new BusinessException("El teléfono solo puede contener dígitos, espacios y los caracteres: + - ( )");

            await _unitOfWork.CustomerRepository.Add(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));

            var existing = await _unitOfWork.CustomerRepository.GetById(customer.Id);
            if (existing == null) throw new KeyNotFoundException("El cliente no existe");

            // Validations
            if (string.IsNullOrWhiteSpace(customer.Name) || customer.Name.Length < 3 || customer.Name.Length > 100)
                throw new BusinessException("El nombre debe tener entre 3 y 100 caracteres");

            if (string.IsNullOrWhiteSpace(customer.Email) || !IsValidEmail(customer.Email))
                throw new BusinessException("El email no tiene un formato válido");

            // Check unique email (except for the same customer)
            var existingEmail = await _unitOfWork.CustomerRepository.GetByEmailAsync(customer.Email);
            if (existingEmail != null && existingEmail.Id != customer.Id)
                throw new BusinessException("Ya existe otro cliente con ese email");

            if (string.IsNullOrWhiteSpace(customer.Phone) || customer.Phone.Length < 7 || customer.Phone.Length > 20)
                throw new BusinessException("El teléfono debe tener entre 7 y 20 caracteres");

            if (!Regex.IsMatch(customer.Phone, @"^[\d\s\-\+\(\)]+$"))
                throw new BusinessException("El teléfono solo puede contener dígitos, espacios y los caracteres: + - ( )");

            await _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.CustomerRepository.GetById(id);
            if (existing == null) throw new KeyNotFoundException("El cliente no existe");

            // Cannot delete customer with active shipments
            var hasActiveShipments = await _unitOfWork.CustomerRepository.HasActiveShipmentsAsync(id);
            if (hasActiveShipments)
                throw new BusinessException("No se puede eliminar un cliente con envíos activos. Complete o cancele los envíos primero.");

            await _unitOfWork.CustomerRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
