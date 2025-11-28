using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(VehicleQueryFilter filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            if (filters.PageNumber <= 0) filters.PageNumber = 1;
            if (filters.PageSize <= 0) filters.PageSize = 10;

            var vehicles = await _unitOfWork.VehicleRepository.GetAllDapperAsync(filters);

            var pageNumber = filters.PageNumber;
            var pageSize = filters.PageSize;
            var sourceList = vehicles.ToList();
            var count = sourceList.Count;
            var items = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var itemsAsObject = items.Cast<object>().ToList();
            var paged = new PagedList<object>(itemsAsObject, count, pageNumber, pageSize);

            return new ResponseData
            {
                Messages = new[] { new Message { Type = "Information", Description = $"Se recuperaron {items.Count} vehículos correctamente" } },
                Pagination = paged,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<IEnumerable<Vehicle>> GetAllDapperAsync()
        {
            return await _unitOfWork.VehicleRepository.GetRecentVehiclesAsync(10);
        }

        public async Task<Vehicle> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del vehículo debe ser mayor a 0");

            var vehicle = await _unitOfWork.VehicleRepository.GetById(id);
            if (vehicle == null)
                throw new KeyNotFoundException($"El vehículo con ID {id} no existe");

            return vehicle;
        }

        public async Task<Vehicle> GetByIdDapperAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del vehículo debe ser mayor a 0");

            var vehicle = await _unitOfWork.VehicleRepository.GetByIdDapperAsync(id);
            if (vehicle == null)
                throw new KeyNotFoundException($"El vehículo con ID {id} no existe");

            return vehicle;
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _unitOfWork.VehicleRepository.GetAvailableVehiclesAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesRequiringMaintenanceAsync()
        {
            return await _unitOfWork.VehicleRepository.GetVehiclesRequiringMaintenanceAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetByCapacityAsync(double requiredWeight, double requiredVolume)
        {
            if (requiredWeight < 0)
                throw new ArgumentException("El peso requerido no puede ser negativo", nameof(requiredWeight));

            if (requiredVolume < 0)
                throw new ArgumentException("El volumen requerido no puede ser negativo", nameof(requiredVolume));

            return await _unitOfWork.VehicleRepository.GetByCapacityAsync(requiredWeight, requiredVolume);
        }

        public async Task<VehicleStatisticsResponse> GetVehicleStatisticsAsync(int vehicleId)
        {
            if (vehicleId <= 0)
                throw new ArgumentException("El ID del vehículo debe ser mayor a 0", nameof(vehicleId));

            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException($"El vehículo con ID {vehicleId} no existe");

            return await _unitOfWork.VehicleRepository.GetVehicleStatisticsAsync(vehicleId);
        }

        public async Task InsertAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            // ========== VALIDACIONES DE NEGOCIO ==========

            // 1. Validar número de placa
            if (string.IsNullOrWhiteSpace(vehicle.PlateNumber))
                throw new BusinessException("El número de placa es requerido");

            if (vehicle.PlateNumber.Length < 5 || vehicle.PlateNumber.Length > 20)
                throw new BusinessException("El número de placa debe tener entre 5 y 20 caracteres");

            var existingPlate = await _unitOfWork.VehicleRepository.GetByPlateNumberAsync(vehicle.PlateNumber);
            if (existingPlate != null)
                throw new BusinessException($"Ya existe un vehículo con la placa {vehicle.PlateNumber}");

            // 2. Validar marca
            if (string.IsNullOrWhiteSpace(vehicle.Brand) || vehicle.Brand.Length < 2)
                throw new BusinessException("La marca debe tener al menos 2 caracteres");

            if (vehicle.Brand.Length > 50)
                throw new BusinessException("La marca no puede exceder 50 caracteres");

            // 3. Validar modelo
            if (string.IsNullOrWhiteSpace(vehicle.Model) || vehicle.Model.Length < 2)
                throw new BusinessException("El modelo debe tener al menos 2 caracteres");

            if (vehicle.Model.Length > 100)
                throw new BusinessException("El modelo no puede exceder 100 caracteres");

            // 4. Validar año
            if (vehicle.Year < 1900 || vehicle.Year > DateTime.Now.Year + 1)
                throw new BusinessException($"El año debe estar entre 1900 y {DateTime.Now.Year + 1}");

            // 5. Validar capacidades
            if (vehicle.MaxWeightCapacityKg <= 0)
                throw new BusinessException("La capacidad máxima de peso debe ser mayor a 0");

            if (vehicle.MaxWeightCapacityKg > 50000)
                throw new BusinessException("La capacidad máxima de peso no puede exceder 50,000 kg");

            if (vehicle.MaxVolumeCapacityM3 <= 0)
                throw new BusinessException("La capacidad máxima de volumen debe ser mayor a 0");

            if (vehicle.MaxVolumeCapacityM3 > 200)
                throw new BusinessException("La capacidad máxima de volumen no puede exceder 200 m³");

            // 6. Validar cargas actuales
            if (vehicle.CurrentWeightKg < 0)
                throw new BusinessException("El peso actual no puede ser negativo");

            if (vehicle.CurrentWeightKg > vehicle.MaxWeightCapacityKg)
                throw new BusinessException("El peso actual no puede exceder la capacidad máxima");

            if (vehicle.CurrentVolumeM3 < 0)
                throw new BusinessException("El volumen actual no puede ser negativo");

            if (vehicle.CurrentVolumeM3 > vehicle.MaxVolumeCapacityM3)
                throw new BusinessException("El volumen actual no puede exceder la capacidad máxima");

            // 7. Validar kilometraje
            if (vehicle.CurrentMileage < 0)
                throw new BusinessException("El kilometraje no puede ser negativo");

            if (vehicle.LastMaintenanceMileage.HasValue && vehicle.LastMaintenanceMileage < 0)
                throw new BusinessException("El kilometraje del último mantenimiento no puede ser negativo");

            if (vehicle.LastMaintenanceMileage.HasValue && vehicle.LastMaintenanceMileage > vehicle.CurrentMileage)
                throw new BusinessException("El kilometraje del último mantenimiento no puede ser mayor al kilometraje actual");

            // 8. Validar fechas de mantenimiento
            if (vehicle.LastMaintenanceDate.HasValue && vehicle.LastMaintenanceDate > DateTime.Now)
                throw new BusinessException("La fecha del último mantenimiento no puede ser futura");

            if (vehicle.NextMaintenanceDate.HasValue && vehicle.LastMaintenanceDate.HasValue)
            {
                if (vehicle.NextMaintenanceDate <= vehicle.LastMaintenanceDate)
                    throw new BusinessException("La fecha del próximo mantenimiento debe ser posterior al último mantenimiento");
            }

            // 9. Validar consumo de combustible
            if (vehicle.FuelConsumptionPer100Km.HasValue && vehicle.FuelConsumptionPer100Km <= 0)
                throw new BusinessException("El consumo de combustible debe ser mayor a 0");

            // 10. Validar VIN (opcional)
            if (!string.IsNullOrWhiteSpace(vehicle.VIN) && vehicle.VIN.Length != 17)
                throw new BusinessException("El VIN debe tener exactamente 17 caracteres");

            // 11. Validar fecha de vencimiento de seguro
            if (vehicle.InsuranceExpiryDate.HasValue && vehicle.InsuranceExpiryDate <= DateTime.Now)
                throw new BusinessException("El seguro está vencido");

            // 12. Validar fecha de adquisición
            if (vehicle.PurchaseDate > DateTime.Now)
                throw new BusinessException("La fecha de adquisición no puede ser futura");

            // 13. Validar almacén base si existe
            if (vehicle.BaseWarehouseId.HasValue)
            {
                var warehouse = await _unitOfWork.WarehouseRepository.GetById(vehicle.BaseWarehouseId.Value);
                if (warehouse == null)
                    throw new BusinessException("El almacén base no existe");
            }

            // 14. Validar conductor asignado si existe
            if (vehicle.AssignedDriverId.HasValue)
            {
                var driver = await _unitOfWork.DriverRepository.GetById(vehicle.AssignedDriverId.Value);
                if (driver == null)
                    throw new BusinessException("El conductor asignado no existe");

                if (driver.CurrentVehicleId.HasValue && driver.CurrentVehicleId != vehicle.Id)
                    throw new BusinessException("El conductor ya está asignado a otro vehículo");
            }

            // Establecer valores por defecto
            vehicle.IsActive = true;
            vehicle.Status = VehicleStatus.Available;
            vehicle.CreatedAt = DateTime.UtcNow;
            vehicle.CurrentWeightKg = 0;
            vehicle.CurrentVolumeM3 = 0;

            await _unitOfWork.VehicleRepository.Add(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            var existing = await _unitOfWork.VehicleRepository.GetById(vehicle.Id);
            if (existing == null)
                throw new KeyNotFoundException("El vehículo no existe");

            // Aplicar mismas validaciones que en Insert
            if (string.IsNullOrWhiteSpace(vehicle.PlateNumber))
                throw new BusinessException("El número de placa es requerido");

            // Validar unicidad de placa (excepto el mismo vehículo)
            var existingByPlate = await _unitOfWork.VehicleRepository.GetByPlateNumberAsync(vehicle.PlateNumber);
            if (existingByPlate != null && existingByPlate.Id != vehicle.Id)
                throw new BusinessException($"Ya existe otro vehículo con la placa {vehicle.PlateNumber}");

            if (vehicle.MaxWeightCapacityKg <= 0)
                throw new BusinessException("La capacidad máxima de peso debe ser mayor a 0");

            if (vehicle.CurrentWeightKg > vehicle.MaxWeightCapacityKg)
                throw new BusinessException("El peso actual no puede exceder la capacidad máxima");

            if (vehicle.CurrentVolumeM3 > vehicle.MaxVolumeCapacityM3)
                throw new BusinessException("El volumen actual no puede exceder la capacidad máxima");

            await _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.VehicleRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException("El vehículo no existe");

            // No permitir eliminar si tiene conductor asignado
            if (existing.AssignedDriverId.HasValue)
                throw new BusinessException("No se puede eliminar un vehículo con conductor asignado. Primero remueva la asignación.");

            // No permitir eliminar si está en tránsito
            if (existing.Status == VehicleStatus.InTransit)
                throw new BusinessException("No se puede eliminar un vehículo en tránsito");

            await _unitOfWork.VehicleRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var existing = await _unitOfWork.VehicleRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException("El vehículo no existe");

            if (!existing.IsActive)
                throw new BusinessException("El vehículo ya está inactivo");

            if (existing.Status == VehicleStatus.InTransit)
                throw new BusinessException("No se puede desactivar un vehículo en tránsito");

            existing.IsActive = false;
            existing.Status = VehicleStatus.OutOfService;

            await _unitOfWork.VehicleRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCurrentLoadAsync(int vehicleId, double weight, double volume)
        {
            if (vehicleId <= 0)
                throw new ArgumentException("El ID del vehículo debe ser mayor a 0", nameof(vehicleId));

            if (weight < 0)
                throw new ArgumentException("El peso no puede ser negativo", nameof(weight));

            if (volume < 0)
                throw new ArgumentException("El volumen no puede ser negativo", nameof(volume));

            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("El vehículo no existe");

            if (weight > vehicle.MaxWeightCapacityKg)
                throw new BusinessException($"El peso excede la capacidad máxima ({vehicle.MaxWeightCapacityKg} kg)");

            if (volume > vehicle.MaxVolumeCapacityM3)
                throw new BusinessException($"El volumen excede la capacidad máxima ({vehicle.MaxVolumeCapacityM3} m³)");

            await _unitOfWork.VehicleRepository.UpdateCurrentLoadAsync(vehicleId, weight, volume);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignDriverAsync(int vehicleId, int driverId)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("El vehículo no existe");

            if (!vehicle.IsActive)
                throw new BusinessException("No se puede asignar conductor a un vehículo inactivo");

            if (vehicle.Status != VehicleStatus.Available)
                throw new BusinessException("El vehículo no está disponible");

            if (vehicle.AssignedDriverId.HasValue)
                throw new BusinessException("El vehículo ya tiene un conductor asignado");

            var driver = await _unitOfWork.DriverRepository.GetById(driverId);
            if (driver == null)
                throw new KeyNotFoundException("El conductor no existe");

            if (!driver.IsActive)
                throw new BusinessException("No se puede asignar un conductor inactivo");

            if (driver.CurrentVehicleId.HasValue)
                throw new BusinessException("El conductor ya está asignado a otro vehículo");

            // Asignar
            vehicle.AssignedDriverId = driverId;
            driver.CurrentVehicleId = vehicleId;

            await _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.DriverRepository.Update(driver);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnassignDriverAsync(int vehicleId)
        {
            var vehicle = await _unitOfWork.VehicleRepository.GetById(vehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException("El vehículo no existe");

            if (!vehicle.AssignedDriverId.HasValue)
                throw new BusinessException("El vehículo no tiene conductor asignado");

            var driver = await _unitOfWork.DriverRepository.GetById(vehicle.AssignedDriverId.Value);
            if (driver != null)
            {
                driver.CurrentVehicleId = null;
                await _unitOfWork.DriverRepository.Update(driver);
            }

            vehicle.AssignedDriverId = null;
            await _unitOfWork.VehicleRepository.Update(vehicle);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}