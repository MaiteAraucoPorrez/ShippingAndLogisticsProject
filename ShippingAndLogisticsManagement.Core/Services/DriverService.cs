using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
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


            // Validar nombre completo
            if (string.IsNullOrWhiteSpace(driver.FullName) || driver.FullName.Length < 5)
                throw new BusinessException("El nombre completo debe tener al menos 5 caracteres");

            if (driver.FullName.Length > 100)
                throw new BusinessException("El nombre completo no puede exceder 100 caracteres");

            // Validar número de licencia
            if (string.IsNullOrWhiteSpace(driver.LicenseNumber))
                throw new BusinessException("El número de licencia es requerido");

            var existingByLicense = await _unitOfWork.DriverRepository.GetByLicenseNumberAsync(driver.LicenseNumber);
            if (existingByLicense != null)
                throw new BusinessException("Ya existe un conductor con ese número de licencia");

            // Validar teléfono
            if (string.IsNullOrWhiteSpace(driver.Phone) || driver.Phone.Length < 7 || driver.Phone.Length > 20)
                throw new BusinessException("El teléfono debe tener entre 7 y 20 caracteres");

            if (!Regex.IsMatch(driver.Phone, @"^[\d\s\-\+\(\)]+$"))
                throw new BusinessException("El teléfono solo puede contener dígitos, espacios y los caracteres: + - ( )");

            // Validar email
            if (string.IsNullOrWhiteSpace(driver.Email) || !IsValidEmail(driver.Email))
                throw new BusinessException("El email no tiene un formato válido");

            // Validar fecha de nacimiento
            if (driver.DateOfBirth >= DateTime.Now)
                throw new BusinessException("La fecha de nacimiento debe ser pasada");

            var age = DateTime.Now.Year - driver.DateOfBirth.Year;
            if (age < 18)
                throw new BusinessException("El conductor debe ser mayor de 18 años");

            // Establecer valores por defecto
            driver.IsActive = true;
            driver.Status = DriverStatus.Available;
            driver.TotalDeliveries = 0;

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

            // Aplicar validaciones
            if (string.IsNullOrWhiteSpace(driver.FullName) || driver.FullName.Length < 5)
                throw new BusinessException("El nombre completo debe tener al menos 5 caracteres");

            if (driver.FullName.Length > 100)
                throw new BusinessException("El nombre completo no puede exceder 100 caracteres");

            // Validar unicidad de licencia (excluyendo el mismo conductor)
            var existingByLicense = await _unitOfWork.DriverRepository.GetByLicenseNumberAsync(driver.LicenseNumber);
            if (existingByLicense != null && existingByLicense.Id != driver.Id)
                throw new BusinessException("Ya existe otro conductor con ese número de licencia");

            // Validar email
            if (!IsValidEmail(driver.Email))
                throw new BusinessException("El email no tiene un formato válido");

            // Validar teléfono
            if (string.IsNullOrWhiteSpace(driver.Phone) || driver.Phone.Length < 7 || driver.Phone.Length > 20)
                throw new BusinessException("El teléfono debe tener entre 7 y 20 caracteres");

            if (!System.Text.RegularExpressions.Regex.IsMatch(driver.Phone, @"^[\d\s\-\+\(\)]+$"))
                throw new BusinessException("El teléfono solo puede contener dígitos, espacios y los caracteres: + - ( )");

            // Validar fecha de nacimiento
            if (driver.DateOfBirth >= DateTime.Now)
                throw new BusinessException("La fecha de nacimiento debe ser pasada");

            var age = DateTime.Now.Year - driver.DateOfBirth.Year;
            if (driver.DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;
            if (age < 18)
                throw new BusinessException("El conductor debe ser mayor de 18 años");

            existing.FullName = driver.FullName;
            existing.LicenseNumber = driver.LicenseNumber;
            existing.Phone = driver.Phone;
            existing.Email = driver.Email;
            existing.Address = driver.Address;
            existing.City = driver.City;
            existing.DateOfBirth = driver.DateOfBirth;
            existing.IsActive = driver.IsActive;
            existing.Status = driver.Status;
            existing.CurrentVehicleId = driver.CurrentVehicleId;
            existing.TotalDeliveries = driver.TotalDeliveries;

            await _unitOfWork.DriverRepository.Update(existing);
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