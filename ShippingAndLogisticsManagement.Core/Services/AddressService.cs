using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Departamentos válidos de Bolivia
        private static readonly string[] ValidDepartments = new[]
        {
            "La Paz", "Cochabamba", "Santa Cruz", "Oruro", "Potosí",
            "Chuquisaca", "Tarija", "Beni", "Pando"
        };

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(AddressQueryFilter filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            // Valores por defecto para paginación
            if (filters.PageNumber <= 0) filters.PageNumber = 1;
            if (filters.PageSize <= 0) filters.PageSize = 10;

            var addresses = await _unitOfWork.AddressRepository.GetAllDapperAsync(filters);

            // Crear PagedList
            var pageNumber = filters.PageNumber;
            var pageSize = filters.PageSize;
            var sourceList = addresses.ToList();
            var count = sourceList.Count;
            var items = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Building PagedList<object> para ResponseData
            var itemsAsObject = items.Cast<object>().ToList();
            var paged = new PagedList<object>(itemsAsObject, count, pageNumber, pageSize);

            var messages = new Message[]
            {
                new()
                {
                    Type = "Information",
                    Description = $"Se recuperaron {items.Count} direcciones correctamente"
                }
            };

            return new ResponseData()
            {
                Messages = messages,
                Pagination = paged,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<IEnumerable<Address>> GetByCustomerIdAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("El ID del cliente debe ser mayor a 0", nameof(customerId));

            // Verificar que el cliente existe
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"El cliente con ID {customerId} no existe");

            var addresses = await _unitOfWork.AddressRepository.GetByCustomerIdAsync(customerId);
            return addresses;
        }

        public async Task<IEnumerable<Address>> GetAllDapperAsync()
        {
            var addresses = await _unitOfWork.AddressRepository.GetRecentAddressesAsync(10);
            return addresses;
        }

        public async Task<Address> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID de la dirección debe ser mayor a 0");

            var address = await _unitOfWork.AddressRepository.GetById(id);
            if (address == null)
                throw new KeyNotFoundException($"La dirección con ID {id} no existe");

            return address;
        }

        public async Task<Address> GetByIdDapperAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID de la dirección debe ser mayor a 0");

            var address = await _unitOfWork.AddressRepository.GetByIdDapperAsync(id);
            if (address == null)
                throw new KeyNotFoundException($"La dirección con ID {id} no existe");

            return address;
        }

        public async Task<Address> GetDefaultAddressAsync(int customerId, AddressType type)
        {
            if (customerId <= 0)
                throw new ArgumentException("El ID del cliente debe ser mayor a 0", nameof(customerId));

            // Verificar que el cliente existe
            var customer = await _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer == null)
                throw new KeyNotFoundException($"El cliente con ID {customerId} no existe");

            var address = await _unitOfWork.AddressRepository.GetDefaultAddressAsync(customerId, type);

            if (address == null)
                throw new KeyNotFoundException($"El cliente no tiene una dirección predeterminada de tipo {type}");

            return address;
        }

        public async Task InsertAsync(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            // ========== VALIDACIONES DE NEGOCIO ==========

            // 1. Verificar que el cliente existe
            var customer = await _unitOfWork.CustomerRepository.GetById(address.CustomerId);
            if (customer == null)
                throw new BusinessException("El cliente asociado no existe");

            // 2. Validar calle
            if (string.IsNullOrWhiteSpace(address.Street) || address.Street.Length < 5)
                throw new BusinessException("La dirección debe tener al menos 5 caracteres");

            if (address.Street.Length > 200)
                throw new BusinessException("La dirección no puede exceder 200 caracteres");

            // 3. Validar ciudad
            if (string.IsNullOrWhiteSpace(address.City) || address.City.Length < 3)
                throw new BusinessException("La ciudad debe tener al menos 3 caracteres");

            if (address.City.Length > 100)
                throw new BusinessException("La ciudad no puede exceder 100 caracteres");

            // 4. Validar departamento
            if (string.IsNullOrWhiteSpace(address.Department))
                throw new BusinessException("El departamento es requerido");

            if (!ValidDepartments.Contains(address.Department, StringComparer.OrdinalIgnoreCase))
                throw new BusinessException(
                    $"El departamento '{address.Department}' no es válido. " +
                    $"Debe ser uno de: {string.Join(", ", ValidDepartments)}");

            // 5. Validar zona (opcional pero si existe debe ser válida)
            if (!string.IsNullOrWhiteSpace(address.Zone) && address.Zone.Length > 100)
                throw new BusinessException("La zona no puede exceder 100 caracteres");

            // 6. Validar código postal (opcional)
            if (!string.IsNullOrWhiteSpace(address.PostalCode) && address.PostalCode.Length > 20)
                throw new BusinessException("El código postal no puede exceder 20 caracteres");

            // 7. Validar referencia (opcional)
            if (!string.IsNullOrWhiteSpace(address.Reference) && address.Reference.Length > 500)
                throw new BusinessException("La referencia no puede exceder 500 caracteres");

            // 8. Validar alias (opcional)
            if (!string.IsNullOrWhiteSpace(address.Alias) && address.Alias.Length > 50)
                throw new BusinessException("El alias no puede exceder 50 caracteres");

            // 9. Validar nombre de contacto (opcional)
            if (!string.IsNullOrWhiteSpace(address.ContactName) && address.ContactName.Length > 100)
                throw new BusinessException("El nombre de contacto no puede exceder 100 caracteres");

            // 10. Validar teléfono de contacto (opcional)
            if (!string.IsNullOrWhiteSpace(address.ContactPhone))
            {
                if (address.ContactPhone.Length < 7 || address.ContactPhone.Length > 20)
                    throw new BusinessException("El teléfono de contacto debe tener entre 7 y 20 caracteres");

                // Validar que solo contenga dígitos, espacios y caracteres permitidos
                if (!System.Text.RegularExpressions.Regex.IsMatch(address.ContactPhone, @"^[\d\s\-\+\(\)]+$"))
                    throw new BusinessException("El teléfono de contacto solo puede contener dígitos y caracteres: + - ( )");
            }

            // 11. Validar coordenadas GPS (opcional pero deben ser válidas)
            if (address.Latitude.HasValue)
            {
                if (address.Latitude.Value < -90 || address.Latitude.Value > 90)
                    throw new BusinessException("La latitud debe estar entre -90 y 90");
            }

            if (address.Longitude.HasValue)
            {
                if (address.Longitude.Value < -180 || address.Longitude.Value > 180)
                    throw new BusinessException("La longitud debe estar entre -180 y 180");
            }

            // 12. Límite de direcciones por cliente (máximo 10)
            var addressCount = await _unitOfWork.AddressRepository.CountActiveAddressesAsync(address.CustomerId);
            if (addressCount >= 10)
                throw new BusinessException("El cliente ha alcanzado el límite máximo de 10 direcciones activas");

            // 13. Si se marca como predeterminada, desmarcar las demás
            if (address.IsDefault)
            {
                await _unitOfWork.AddressRepository.UnsetDefaultAddressesAsync(
                    address.CustomerId,
                    address.Type);
            }
            else
            {
                // Si no tiene direcciones predeterminadas de este tipo, forzar esta como predeterminada
                var hasDefault = await _unitOfWork.AddressRepository.HasDefaultAddressAsync(
                    address.CustomerId,
                    address.Type);

                if (!hasDefault)
                {
                    address.IsDefault = true;
                }
            }

            // Establecer valores por defecto
            address.IsActive = true;
            address.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.AddressRepository.Add(address);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Address address)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            // Verificar que la dirección existe
            var existing = await _unitOfWork.AddressRepository.GetById(address.Id);
            if (existing == null)
                throw new KeyNotFoundException("La dirección no existe");

            // Verificar que el cliente existe
            var customer = await _unitOfWork.CustomerRepository.GetById(address.CustomerId);
            if (customer == null)
                throw new BusinessException("El cliente asociado no existe");

            // Aplicar las mismas validaciones que en Insert
            if (string.IsNullOrWhiteSpace(address.Street) || address.Street.Length < 5)
                throw new BusinessException("La dirección debe tener al menos 5 caracteres");

            if (address.Street.Length > 200)
                throw new BusinessException("La dirección no puede exceder 200 caracteres");

            if (string.IsNullOrWhiteSpace(address.City) || address.City.Length < 3)
                throw new BusinessException("La ciudad debe tener al menos 3 caracteres");

            if (!ValidDepartments.Contains(address.Department, StringComparer.OrdinalIgnoreCase))
                throw new BusinessException(
                    $"El departamento '{address.Department}' no es válido. " +
                    $"Debe ser uno de: {string.Join(", ", ValidDepartments)}");

            if (!string.IsNullOrWhiteSpace(address.ContactPhone))
            {
                if (address.ContactPhone.Length < 7 || address.ContactPhone.Length > 20)
                    throw new BusinessException("El teléfono de contacto debe tener entre 7 y 20 caracteres");

                if (!System.Text.RegularExpressions.Regex.IsMatch(address.ContactPhone, @"^[\d\s\-\+\(\)]+$"))
                    throw new BusinessException("El teléfono de contacto solo puede contener dígitos y caracteres: + - ( )");
            }

            if (address.Latitude.HasValue && (address.Latitude.Value < -90 || address.Latitude.Value > 90))
                throw new BusinessException("La latitud debe estar entre -90 y 90");

            if (address.Longitude.HasValue && (address.Longitude.Value < -180 || address.Longitude.Value > 180))
                throw new BusinessException("La longitud debe estar entre -180 y 180");

            // Si se marca como predeterminada, desmarcar las demás
            if (address.IsDefault && !existing.IsDefault)
            {
                await _unitOfWork.AddressRepository.UnsetDefaultAddressesAsync(
                    address.CustomerId,
                    address.Type,
                    address.Id);
            }

            // Si se desmarca y era la única predeterminada, no permitir
            if (!address.IsDefault && existing.IsDefault)
            {
                var hasOtherDefault = await _unitOfWork.AddressRepository.HasDefaultAddressAsync(
                    address.CustomerId,
                    address.Type,
                    address.Id);

                if (!hasOtherDefault)
                    throw new BusinessException(
                        "No se puede desmarcar la única dirección predeterminada. " +
                        "Primero marque otra dirección como predeterminada.");
            }

            await _unitOfWork.AddressRepository.Update(address);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.AddressRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException("La dirección no existe");

            // Verificar que no sea la única dirección predeterminada activa
            if (existing.IsDefault && existing.IsActive)
            {
                var hasOtherDefault = await _unitOfWork.AddressRepository.HasDefaultAddressAsync(
                    existing.CustomerId,
                    existing.Type,
                    existing.Id);

                if (!hasOtherDefault)
                    throw new BusinessException(
                        "No se puede eliminar la única dirección predeterminada activa. " +
                        "Primero marque otra dirección como predeterminada.");
            }

            await _unitOfWork.AddressRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var existing = await _unitOfWork.AddressRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException("La dirección no existe");

            if (!existing.IsActive)
                throw new BusinessException("La dirección ya está inactiva");

            // Verificar que no sea la única dirección predeterminada activa
            if (existing.IsDefault)
            {
                var hasOtherDefault = await _unitOfWork.AddressRepository.HasDefaultAddressAsync(
                    existing.CustomerId,
                    existing.Type,
                    existing.Id);

                if (!hasOtherDefault)
                    throw new BusinessException(
                        "No se puede desactivar la única dirección predeterminada. " +
                        "Primero marque otra dirección como predeterminada.");
            }

            existing.IsActive = false;
            await _unitOfWork.AddressRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetAsDefaultAsync(int id)
        {
            var address = await _unitOfWork.AddressRepository.GetById(id);
            if (address == null)
                throw new KeyNotFoundException("La dirección no existe");

            if (!address.IsActive)
                throw new BusinessException("No se puede marcar como predeterminada una dirección inactiva");

            if (address.IsDefault)
                throw new BusinessException("La dirección ya es predeterminada");

            // Desmarcar otras direcciones predeterminadas del mismo tipo
            await _unitOfWork.AddressRepository.UnsetDefaultAddressesAsync(
                address.CustomerId,
                address.Type,
                address.Id);

            // Marcar como predeterminada
            address.IsDefault = true;
            await _unitOfWork.AddressRepository.Update(address);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}