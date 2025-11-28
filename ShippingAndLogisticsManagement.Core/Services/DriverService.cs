using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;
using System.Text.RegularExpressions;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class DriverService : IDriverService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DriverService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(DriverQueryFilter filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            // Valores por defecto
            if (filters.PageNumber <= 0) filters.PageNumber = 1;
            if (filters.PageSize <= 0) filters.PageSize = 10;

            var drivers = await _unitOfWork.DriverRepository.GetAllDapperAsync(filters);

            // Crear PagedList
            var pageNumber = filters.PageNumber;
            var pageSize = filters.PageSize;
            var sourceList = drivers.ToList();
            var count = sourceList.Count;
            var items = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var itemsAsObject = items.Cast<object>().ToList();
            var paged = new PagedList<object>(itemsAsObject, count, pageNumber, pageSize);

            return new ResponseData
            {
                Messages = new[] { new Message { Type = "Information", Description = $"Se recuperaron {items.Count} conductores correctamente" } },
                Pagination = paged,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<IEnumerable<Driver>> GetAllDapperAsync()
        {
            return await _unitOfWork.DriverRepository.GetRecentDriversAsync(10);
        }

        public async Task<Driver> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del conductor debe ser mayor a 0");

            var driver = await _unitOfWork.DriverRepository.GetById(id);
            if (driver == null)
                throw new KeyNotFoundException($"El conductor con ID {id} no existe");

            return driver;
        }

        public async Task<Driver> GetByIdDapperAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del conductor debe ser mayor a 0");

            var driver = await _unitOfWork.DriverRepository.GetByIdDapperAsync(id);
            if (driver == null)
                throw new KeyNotFoundException($"El conductor con ID {id} no existe");

            return driver;
        }

        public async Task<IEnumerable<Driver>> GetAvailableDriversAsync()
        {
            return await _unitOfWork.DriverRepository.GetAvailableDriversAsync();
        }

        public async Task<IEnumerable<Driver>> GetDriversWithExpiringLicensesAsync(int daysThreshold = 30)
        {
            if (daysThreshold <= 0)
                throw new ArgumentException("El umbral de días debe ser mayor a 0", nameof(daysThreshold));

            return await _unitOfWork.DriverRepository.GetDriversWithExpiringLicensesAsync(daysThreshold);
        }

        public async Task<DriverStatisticsResponse> GetDriverStatisticsAsync(int driverId)
        {
            if (driverId <= 0)
                throw new ArgumentException("El ID del conductor debe ser mayor a 0", nameof(driverId));

            var driver = await _unitOfWork.DriverRepository.GetById(driverId);
            if (driver == null)
                throw new KeyNotFoundException($"El conductor con ID {driverId} no existe");

            return await _unitOfWork.DriverRepository.GetDriverStatisticsAsync(driverId);
        }

        public async Task InsertAsync(Driver driver)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            // ========== VALIDACIONES DE NEGOCIO ==========

            // 1. Validar nombre completo
            if (string.IsNullOrWhiteSpace(driver.FullName) || driver.FullName.Length < 5)
                throw new BusinessException("El nombre completo debe tener al menos 5 caracteres");

            if (driver.FullName.Length > 100)
                throw new BusinessException("El nombre completo no puede exceder 100 caracteres");

            // 2. Validar documento de identidad
            if (string.IsNullOrWhiteSpace(driver.IdentityDocument))
                throw new BusinessException("El documento de identidad es requerido");

            var existingByIdentity = await _unitOfWork.DriverRepository.GetByIdentityDocumentAsync(driver.IdentityDocument);
            if (existingByIdentity != null)
                throw new BusinessException("Ya existe un conductor con ese documento de identidad");

            // 3. Validar número de licencia
            if (string.IsNullOrWhiteSpace(driver.LicenseNumber))
                throw new BusinessException("El número de licencia es requerido");

            var existingByLicense = await _unitOfWork.DriverRepository.GetByLicenseNumberAsync(driver.LicenseNumber);
            if (existingByLicense != null)
                throw new BusinessException("Ya existe un conductor con ese número de licencia");

            // 4. Validar categoría de licencia
            if (string.IsNullOrWhiteSpace(driver.LicenseCategory))
                throw new BusinessException("La categoría de licencia es requerida");

            // 5. Validar fechas de licencia
            if (driver.LicenseIssueDate > DateTime.Now)
                throw new BusinessException("La fecha de emisión de licencia no puede ser futura");

            if (driver.LicenseExpiryDate <= DateTime.Now)
                throw new BusinessException("La licencia está vencida");

            if (driver.LicenseExpiryDate <= driver.LicenseIssueDate)
                throw new BusinessException("La fecha de vencimiento debe ser posterior a la fecha de emisión");

            // 6. Validar teléfono
            if (string.IsNullOrWhiteSpace(driver.Phone) || driver.Phone.Length < 7 || driver.Phone.Length > 20)
                throw new BusinessException("El teléfono debe tener entre 7 y 20 caracteres");

            if (!Regex.IsMatch(driver.Phone, @"^[\d\s\-\+\(\)]+$"))
                throw new BusinessException("El teléfono solo puede contener dígitos, espacios y los caracteres: + - ( )");

            // 7. Validar email
            if (string.IsNullOrWhiteSpace(driver.Email) || !IsValidEmail(driver.Email))
                throw new BusinessException("El email no tiene un formato válido");

            // 8. Validar fecha de nacimiento
            if (driver.DateOfBirth >= DateTime.Now)
                throw new BusinessException("La fecha de nacimiento debe ser pasada");

            var age = DateTime.Now.Year - driver.DateOfBirth.Year;
            if (age < 18)
                throw new BusinessException("El conductor debe ser mayor de 18 años");

            // 9. Validar fecha de contratación
            if (driver.HireDate > DateTime.Now)
                throw new BusinessException("La fecha de contratación no puede ser futura");

            // 10. Validar años de experiencia
            if (driver.YearsOfExperience < 0)
                throw new BusinessException("Los años de experiencia no pueden ser negativos");

            // Establecer valores por defecto
            driver.IsActive = true;
            driver.Status = DriverStatus.Available;
            driver.TotalDeliveries = 0;
            driver.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.DriverRepository.Add(driver);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Driver driver)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            var existing = await _unitOfWork.DriverRepository.GetById(driver.Id);
            if (existing == null)
                throw new KeyNotFoundException("El conductor no existe");

            // Aplicar mismas validaciones que en Insert
            if (string.IsNullOrWhiteSpace(driver.FullName) || driver.FullName.Length < 5)
                throw new BusinessException("El nombre completo debe tener al menos 5 caracteres");

            // Validar unicidad de documentos (excepto el mismo conductor)
            var existingByIdentity = await _unitOfWork.DriverRepository.GetByIdentityDocumentAsync(driver.IdentityDocument);
            if (existingByIdentity != null && existingByIdentity.Id != driver.Id)
                throw new BusinessException("Ya existe otro conductor con ese documento de identidad");

            var existingByLicense = await _unitOfWork.DriverRepository.GetByLicenseNumberAsync(driver.LicenseNumber);
            if (existingByLicense != null && existingByLicense.Id != driver.Id)
                throw new BusinessException("Ya existe otro conductor con ese número de licencia");

            if (driver.LicenseExpiryDate <= DateTime.Now)
                throw new BusinessException("La licencia está vencida");

            if (!IsValidEmail(driver.Email))
                throw new BusinessException("El email no tiene un formato válido");

            await _unitOfWork.DriverRepository.Update(driver);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.DriverRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException("El conductor no existe");

            // No permitir eliminar si tiene vehículo asignado
            if (existing.CurrentVehicleId.HasValue)
                throw new BusinessException("No se puede eliminar un conductor con vehículo asignado. Primero remueva la asignación.");

            await _unitOfWork.DriverRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var existing = await _unitOfWork.DriverRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException("El conductor no existe");

            if (!existing.IsActive)
                throw new BusinessException("El conductor ya está inactivo");

            existing.IsActive = false;
            existing.Status = DriverStatus.OffDuty;

            await _unitOfWork.DriverRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignVehicleAsync(int driverId, int vehicleId)
        {
            var driver = await _unitOfWork.DriverRepository.GetById(driverId);
            if (driver == null)
                throw new KeyNotFoundException("El conductor no existe");

            if (!driver.IsActive)
                throw new BusinessException("No se puede asignar vehículo a un conductor inactivo");

            if (driver.Status != DriverStatus.Available)
                throw new BusinessException("El conductor no está disponible para asignación");

            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("El vehículo no existe");

            if (!vehicle.IsActive)
                throw new BusinessException("No se puede asignar un vehículo inactivo");

            if (vehicle.Status != VehicleStatus.Available)
                throw new BusinessException("El vehículo no está disponible");

            if (vehicle.AssignedDriverId.HasValue)
                throw new BusinessException("El vehículo ya tiene un conductor asignado");

            // Asignar
            driver.CurrentVehicleId = vehicleId;
            vehicle.AssignedDriverId = driverId;

            await _unitOfWork.DriverRepository.Update(driver);
            await _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnassignVehicleAsync(int driverId)
        {
            var driver = await _unitOfWork.DriverRepository.GetById(driverId);
            if (driver == null)
                throw new KeyNotFoundException("El conductor no existe");

            if (!driver.CurrentVehicleId.HasValue)
                throw new BusinessException("El conductor no tiene vehículo asignado");

            var vehicle = await _unitOfWork.VehicleRepository.GetById(driver.CurrentVehicleId.Value);
            if (vehicle != null)
            {
                vehicle.AssignedDriverId = null;
                await _unitOfWork.VehicleRepository.Update(vehicle);
            }

            driver.CurrentVehicleId = null;
            await _unitOfWork.DriverRepository.Update(driver);
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