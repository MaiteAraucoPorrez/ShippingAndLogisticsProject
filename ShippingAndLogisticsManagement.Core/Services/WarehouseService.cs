using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Departamentos válidos de Bolivia
        private static readonly string[] ValidDepartments = new[]
        {
            "La Paz", "Cochabamba", "Santa Cruz", "Oruro", "Potosí",
            "Chuquisaca", "Tarija", "Beni", "Pando"
        };

        public WarehouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(WarehouseQueryFilter filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            // Valores por defecto para paginación
            if (filters.PageNumber <= 0) filters.PageNumber = 1;
            if (filters.PageSize <= 0) filters.PageSize = 10;

            var warehouses = await _unitOfWork.WarehouseRepository.GetAllDapperAsync(filters);

            // Crear PagedList
            var pageNumber = filters.PageNumber;
            var pageSize = filters.PageSize;
            var sourceList = warehouses.ToList();
            var count = sourceList.Count;
            var items = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var itemsAsObject = items.Cast<object>().ToList();
            var paged = new PagedList<object>(itemsAsObject, count, pageNumber, pageSize);

            var messages = new Message[]
            {
                new()
                {
                    Type = "Information",
                    Description = $"Se recuperaron {items.Count} almacenes correctamente"
                }
            };

            return new ResponseData()
            {
                Messages = messages,
                Pagination = paged,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<IEnumerable<Warehouse>> GetAllDapperAsync()
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetRecentWarehousesAsync(10);
            return warehouses;
        }

        public async Task<Warehouse> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del almacén debe ser mayor a 0");

            var warehouse = await _unitOfWork.WarehouseRepository.GetById(id);
            if (warehouse == null)
                throw new KeyNotFoundException($"El almacén con ID {id} no existe");

            return warehouse;
        }

        public async Task<Warehouse> GetByIdDapperAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del almacén debe ser mayor a 0");

            var warehouse = await _unitOfWork.WarehouseRepository.GetByIdDapperAsync(id);
            if (warehouse == null)
                throw new KeyNotFoundException($"El almacén con ID {id} no existe");

            return warehouse;
        }

        public async Task<Warehouse> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("El código del almacén es requerido", nameof(code));

            var warehouse = await _unitOfWork.WarehouseRepository.GetByCodeAsync(code);
            if (warehouse == null)
                throw new KeyNotFoundException($"No existe un almacén con código {code}");

            return warehouse;
        }

        public async Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync()
        {
            var warehouses = await _unitOfWork.WarehouseRepository.GetActiveWarehousesAsync();
            return warehouses;
        }

        public async Task<IEnumerable<Warehouse>> GetAvailableWarehousesAsync(double requiredCapacity)
        {
            if (requiredCapacity <= 0)
                throw new ArgumentException("La capacidad requerida debe ser mayor a 0", nameof(requiredCapacity));

            var warehouses = await _unitOfWork.WarehouseRepository.GetAvailableWarehousesAsync(requiredCapacity);
            return warehouses;
        }

        public async Task<WarehouseStatisticsResponse> GetWarehouseStatisticsAsync(int warehouseId)
        {
            if (warehouseId <= 0)
                throw new ArgumentException("El ID del almacén debe ser mayor a 0", nameof(warehouseId));

            // Verificar que el almacén existe
            var warehouse = await _unitOfWork.WarehouseRepository.GetById(warehouseId);
            if (warehouse == null)
                throw new KeyNotFoundException($"El almacén con ID {warehouseId} no existe");

            var statistics = await _unitOfWork.WarehouseRepository.GetWarehouseStatisticsAsync(warehouseId);

            if (statistics == null)
            {
                // Retornar estadísticas vacías si no hay datos
                statistics = new WarehouseStatisticsResponse
                {
                    WarehouseId = warehouseId,
                    WarehouseName = warehouse.Name,
                    Code = warehouse.Code,
                    City = warehouse.City,
                    TotalShipments = 0,
                    CurrentShipments = 0,
                    DispatchedShipments = 0,
                    OccupancyPercentage = (warehouse.CurrentCapacityM3 / warehouse.MaxCapacityM3) * 100,
                    AvailableCapacity = warehouse.MaxCapacityM3 - warehouse.CurrentCapacityM3,
                    AverageStayTimeHours = null
                };
            }

            return statistics;
        }

        public async Task InsertAsync(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse));


            // Validar nombre
            if (string.IsNullOrWhiteSpace(warehouse.Name) || warehouse.Name.Length < 5)
                throw new BusinessException("El nombre del almacén debe tener al menos 5 caracteres");

            if (warehouse.Name.Length > 100)
                throw new BusinessException("El nombre del almacén no puede exceder 100 caracteres");

            // Validar código único
            if (string.IsNullOrWhiteSpace(warehouse.Code))
                throw new BusinessException("El código del almacén es requerido");

            if (warehouse.Code.Length < 3 || warehouse.Code.Length > 20)
                throw new BusinessException("El código debe tener entre 3 y 20 caracteres");

            // Validar formato del código (solo alfanumérico y guiones)
            if (!Regex.IsMatch(warehouse.Code, @"^[a-zA-Z0-9\-]+$"))
                throw new BusinessException("El código solo puede contener letras, números y guiones");

            // Verificar que el código sea único
            var codeExists = await _unitOfWork.WarehouseRepository.CodeExistsAsync(warehouse.Code);
            if (codeExists)
                throw new BusinessException($"Ya existe un almacén con el código {warehouse.Code}");

            // Validar dirección
            if (string.IsNullOrWhiteSpace(warehouse.Address) || warehouse.Address.Length < 10)
                throw new BusinessException("La dirección debe tener al menos 10 caracteres");

            if (warehouse.Address.Length > 300)
                throw new BusinessException("La dirección no puede exceder 300 caracteres");

            // Validar ciudad
            if (string.IsNullOrWhiteSpace(warehouse.City) || warehouse.City.Length < 3)
                throw new BusinessException("La ciudad debe tener al menos 3 caracteres");

            if (warehouse.City.Length > 100)
                throw new BusinessException("La ciudad no puede exceder 100 caracteres");

            // Validar departamento
            if (string.IsNullOrWhiteSpace(warehouse.Department))
                throw new BusinessException("El departamento es requerido");

            if (!ValidDepartments.Contains(warehouse.Department, StringComparer.OrdinalIgnoreCase))
                throw new BusinessException(
                    $"El departamento '{warehouse.Department}' no es válido. " +
                    $"Debe ser uno de: {string.Join(", ", ValidDepartments)}");

            // Validar teléfono
            if (string.IsNullOrWhiteSpace(warehouse.Phone))
                throw new BusinessException("El teléfono es requerido");

            if (warehouse.Phone.Length < 7 || warehouse.Phone.Length > 20)
                throw new BusinessException("El teléfono debe tener entre 7 y 20 caracteres");

            if (!Regex.IsMatch(warehouse.Phone, @"^[\d\s\-\+\(\)]+$"))
                throw new BusinessException("El teléfono solo puede contener dígitos y caracteres: + - ( )");

            // Validar email (opcional)
            if (!string.IsNullOrWhiteSpace(warehouse.Email))
            {
                if (!IsValidEmail(warehouse.Email))
                    throw new BusinessException("El email no tiene un formato válido");

                if (warehouse.Email.Length > 100)
                    throw new BusinessException("El email no puede exceder 100 caracteres");
            }

            // Validar capacidad máxima
            if (warehouse.MaxCapacityM3 <= 0)
                throw new BusinessException("La capacidad máxima debe ser mayor a 0");

            if (warehouse.MaxCapacityM3 > 100000)
                throw new BusinessException("La capacidad máxima no puede exceder 100,000 m³");

            // Validar capacidad actual
            if (warehouse.CurrentCapacityM3 < 0)
                throw new BusinessException("La capacidad actual no puede ser negativa");

            if (warehouse.CurrentCapacityM3 > warehouse.MaxCapacityM3)
                throw new BusinessException("La capacidad actual no puede exceder la capacidad máxima");

            // Establecer valores por defecto
            warehouse.IsActive = true;
            warehouse.CurrentCapacityM3 = 0; // Siempre inicia vacío

            await _unitOfWork.WarehouseRepository.Add(warehouse);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Warehouse warehouse)
        {
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse));

            // Verificar que el almacén existe
            var existing = await _unitOfWork.WarehouseRepository.GetById(warehouse.Id);
            if (existing == null)
                throw new KeyNotFoundException("El almacén no existe");

            // Aplicar las mismas validaciones que en Insert
            if (string.IsNullOrWhiteSpace(warehouse.Name) || warehouse.Name.Length < 5)
                throw new BusinessException("El nombre del almacén debe tener al menos 5 caracteres");

            if (warehouse.Name.Length > 100)
                throw new BusinessException("El nombre del almacén no puede exceder 100 caracteres");

            // Validar código único (excluyendo el actual)
            if (string.IsNullOrWhiteSpace(warehouse.Code))
                throw new BusinessException("El código del almacén es requerido");

            if (!Regex.IsMatch(warehouse.Code, @"^[a-zA-Z0-9\-]+$"))
                throw new BusinessException("El código solo puede contener letras, números y guiones");

            var codeExists = await _unitOfWork.WarehouseRepository.CodeExistsAsync(warehouse.Code, warehouse.Id);
            if (codeExists)
                throw new BusinessException($"Ya existe otro almacén con el código {warehouse.Code}");

            if (string.IsNullOrWhiteSpace(warehouse.Address) || warehouse.Address.Length < 10)
                throw new BusinessException("La dirección debe tener al menos 10 caracteres");

            if (!ValidDepartments.Contains(warehouse.Department, StringComparer.OrdinalIgnoreCase))
                throw new BusinessException(
                    $"El departamento '{warehouse.Department}' no es válido. " +
                    $"Debe ser uno de: {string.Join(", ", ValidDepartments)}");

            if (warehouse.Phone.Length < 7 || warehouse.Phone.Length > 20)
                throw new BusinessException("El teléfono debe tener entre 7 y 20 caracteres");

            if (!Regex.IsMatch(warehouse.Phone, @"^[\d\s\-\+\(\)]+$"))
                throw new BusinessException("El teléfono solo puede contener dígitos y caracteres: + - ( )");

            if (!string.IsNullOrWhiteSpace(warehouse.Email) && !IsValidEmail(warehouse.Email))
                throw new BusinessException("El email no tiene un formato válido");

            if (warehouse.MaxCapacityM3 <= 0)
                throw new BusinessException("La capacidad máxima debe ser mayor a 0");

            if (warehouse.CurrentCapacityM3 < 0)
                throw new BusinessException("La capacidad actual no puede ser negativa");

            if (warehouse.CurrentCapacityM3 > warehouse.MaxCapacityM3)
                throw new BusinessException("La capacidad actual no puede exceder la capacidad máxima");

            // No permitir cambiar MaxCapacityM3 si hay envíos activos y la nueva capacidad es menor que la actual usada
            if (warehouse.MaxCapacityM3 < warehouse.CurrentCapacityM3)
                throw new BusinessException(
                    $"No se puede reducir la capacidad máxima por debajo de la capacidad actual utilizada ({warehouse.CurrentCapacityM3} m³)");

            existing.Name = warehouse.Name;
            existing.Code = warehouse.Code;
            existing.Address = warehouse.Address;
            existing.City = warehouse.City;
            existing.Department = warehouse.Department;
            existing.Phone = warehouse.Phone;
            existing.Email = warehouse.Email;
            existing.MaxCapacityM3 = warehouse.MaxCapacityM3;
            existing.CurrentCapacityM3 = warehouse.CurrentCapacityM3;
            existing.IsActive = warehouse.IsActive;
            existing.Type = warehouse.Type;
            existing.OperatingHours = warehouse.OperatingHours;
            existing.ManagerName = warehouse.ManagerName;

            await _unitOfWork.WarehouseRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.WarehouseRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException("El almacén no existe");

            // Verificar que no tenga envíos activos
            var hasCurrentShipments = await _unitOfWork.ShipmentWarehouseRepository
                .GetCurrentShipmentsInWarehouseAsync(id);

            if (hasCurrentShipments.Any())
                throw new BusinessException(
                    $"No se puede eliminar el almacén porque tiene {hasCurrentShipments.Count()} envíos activos. " +
                    "Primero despache todos los envíos.");

            await _unitOfWork.WarehouseRepository.Delete(id);
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