using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Enum;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class ShipmentWarehouseService : IShipmentWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShipmentWarehouseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(ShipmentWarehouseQueryFilter filters)
        {
            if (filters == null)
                throw new ArgumentNullException(nameof(filters));

            if (filters.PageNumber <= 0) filters.PageNumber = 1;
            if (filters.PageSize <= 0) filters.PageSize = 10;

            var records = await _unitOfWork.ShipmentWarehouseRepository.GetAll();

            // Aplicar filtros
            var filtered = records.AsEnumerable();

            if (filters.ShipmentId.HasValue)
                filtered = filtered.Where(x => x.ShipmentId == filters.ShipmentId.Value);

            if (filters.WarehouseId.HasValue)
                filtered = filtered.Where(x => x.WarehouseId == filters.WarehouseId.Value);

            if (filters.Status.HasValue)
                filtered = filtered.Where(x => x.Status == filters.Status.Value);

            if (filters.HasExited.HasValue)
            {
                if (filters.HasExited.Value)
                    filtered = filtered.Where(x => x.ExitDate.HasValue);
                else
                    filtered = filtered.Where(x => !x.ExitDate.HasValue);
            }

            if (filters.EntryDateFrom.HasValue)
                filtered = filtered.Where(x => x.EntryDate >= filters.EntryDateFrom.Value);

            if (filters.EntryDateTo.HasValue)
                filtered = filtered.Where(x => x.EntryDate <= filters.EntryDateTo.Value);

            // Paginación
            var pageNumber = filters.PageNumber;
            var pageSize = filters.PageSize;
            var sourceList = filtered.ToList();
            var count = sourceList.Count;
            var items = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var itemsAsObject = items.Cast<object>().ToList();
            var paged = new PagedList<object>(itemsAsObject, count, pageNumber, pageSize);

            return new ResponseData
            {
                Messages = new[] { new Message { Type = "Information", Description = $"Se recuperaron {items.Count} registros" } },
                Pagination = paged,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<ShipmentWarehouse> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID debe ser mayor a 0");

            var record = await _unitOfWork.ShipmentWarehouseRepository.GetById(id);
            if (record == null)
                throw new KeyNotFoundException($"El registro con ID {id} no existe");

            return record;
        }

        public async Task<ShipmentWarehouse> RegisterEntryAsync(ShipmentWarehouse shipmentWarehouse)
        {
            if (shipmentWarehouse == null)
                throw new ArgumentNullException(nameof(shipmentWarehouse));

            // Validar que el envío existe
            var shipment = await _unitOfWork.ShipmentRepository.GetById(shipmentWarehouse.ShipmentId);
            if (shipment == null)
                throw new BusinessException("El envío no existe");

            // Validar que el almacén existe y está activo
            var warehouse = await _unitOfWork.WarehouseRepository.GetById(shipmentWarehouse.WarehouseId);
            if (warehouse == null)
                throw new BusinessException("El almacén no existe");

            if (!warehouse.IsActive)
                throw new BusinessException("El almacén está inactivo");

            // Validar que el envío NO esté ya en otro almacén
            var isInWarehouse = await _unitOfWork.ShipmentWarehouseRepository
                .IsShipmentInWarehouseAsync(shipmentWarehouse.ShipmentId);

            if (isInWarehouse)
                throw new BusinessException("El envío ya está en un almacén. Primero debe registrar su salida.");

            // Validar capacidad disponible
            var availableCapacity = warehouse.MaxCapacityM3 - warehouse.CurrentCapacityM3;
            if (availableCapacity < 1)
                throw new BusinessException("El almacén no tiene capacidad disponible");

            // Establecer valores
            shipmentWarehouse.EntryDate = shipmentWarehouse.EntryDate == default
                ? DateTime.Now
                : shipmentWarehouse.EntryDate;
            shipmentWarehouse.Status = WarehouseShipmentStatus.Received;
            shipmentWarehouse.ExitDate = null;

            await _unitOfWork.ShipmentWarehouseRepository.RegisterEntryAsync(shipmentWarehouse);

            // Actualizar capacidad del almacén
            warehouse.CurrentCapacityM3 += 1;
            await _unitOfWork.WarehouseRepository.Update(warehouse);

            await _unitOfWork.SaveChangesAsync();

            return shipmentWarehouse;
        }

        public async Task RegisterExitAsync(int shipmentWarehouseId, DateTime exitDate, string dispatchedBy)
        {
            if (shipmentWarehouseId <= 0)
                throw new BusinessException("El ID debe ser mayor a 0");

            var shipmentWarehouse = await _unitOfWork.ShipmentWarehouseRepository
                .GetById(shipmentWarehouseId);

            if (shipmentWarehouse == null)
                throw new KeyNotFoundException("El registro no existe");

            if (shipmentWarehouse.ExitDate.HasValue)
                throw new BusinessException("Este envío ya fue despachado anteriormente");

            if (exitDate < shipmentWarehouse.EntryDate)
                throw new BusinessException("La fecha de salida no puede ser anterior a la entrada");

            await _unitOfWork.ShipmentWarehouseRepository.RegisterExitAsync(
                shipmentWarehouseId,
                exitDate,
                dispatchedBy ?? "Sistema"
            );

            // Actualizar capacidad del almacén
            var warehouse = await _unitOfWork.WarehouseRepository
                .GetById(shipmentWarehouse.WarehouseId);

            if (warehouse != null)
            {
                warehouse.CurrentCapacityM3 = Math.Max(0, warehouse.CurrentCapacityM3 - 1);
                await _unitOfWork.WarehouseRepository.Update(warehouse);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShipmentWarehouseHistoryResponse>> GetShipmentHistoryAsync(int shipmentId)
        {
            if (shipmentId <= 0)
                throw new ArgumentException("El ID debe ser mayor a 0", nameof(shipmentId));

            var shipment = await _unitOfWork.ShipmentRepository.GetById(shipmentId);
            if (shipment == null)
                throw new KeyNotFoundException("El envío no existe");

            var history = await _unitOfWork.ShipmentWarehouseRepository
                .GetShipmentHistoryAsync(shipmentId);

            return history;
        }

        public async Task<IEnumerable<ShipmentWarehouse>> GetCurrentShipmentsInWarehouseAsync(int warehouseId)
        {
            if (warehouseId <= 0)
                throw new ArgumentException("El ID debe ser mayor a 0", nameof(warehouseId));

            var warehouse = await _unitOfWork.WarehouseRepository.GetById(warehouseId);
            if (warehouse == null)
                throw new KeyNotFoundException("El almacén no existe");

            return await _unitOfWork.ShipmentWarehouseRepository
                .GetCurrentShipmentsInWarehouseAsync(warehouseId);
        }

        public async Task<ShipmentWarehouse?> GetCurrentLocationAsync(int shipmentId)
        {
            if (shipmentId <= 0)
                throw new ArgumentException("El ID debe ser mayor a 0", nameof(shipmentId));

            return await _unitOfWork.ShipmentWarehouseRepository
                .GetCurrentWarehouseForShipmentAsync(shipmentId);
        }

        public async Task<bool> IsShipmentInWarehouseAsync(int shipmentId)
        {
            if (shipmentId <= 0)
                throw new ArgumentException("El ID debe ser mayor a 0", nameof(shipmentId));

            return await _unitOfWork.ShipmentWarehouseRepository
                .IsShipmentInWarehouseAsync(shipmentId);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.ShipmentWarehouseRepository.GetById(id);
            if (existing == null)
                throw new KeyNotFoundException("El registro no existe");

            // No permitir eliminar si es el registro actual (sin fecha de salida)
            if (!existing.ExitDate.HasValue)
                throw new BusinessException("No se puede eliminar un registro activo. Primero registre la salida del envío.");

            await _unitOfWork.ShipmentWarehouseRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
