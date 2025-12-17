using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.QueryFilters;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del servicio para la gestión de movimientos
    /// de envíos a través de almacenes (check-in/dispatch)
    /// </summary>
    public interface IShipmentWarehouseService
    {
        /// <summary>
        /// Registra la entrada de un envío a un almacén
        /// </summary>
        Task<ShipmentWarehouse> RegisterEntryAsync(ShipmentWarehouse shipmentWarehouse);

        /// <summary>
        /// Registra la salida de un envío de un almacén
        /// </summary>
        Task RegisterExitAsync(int shipmentWarehouseId, DateTime exitDate, string dispatchedBy);

        /// <summary>
        /// Obtiene el historial de almacenes de un envío
        /// </summary>
        Task<IEnumerable<ShipmentWarehouseHistoryResponse>> GetShipmentHistoryAsync(int shipmentId);

        /// <summary>
        /// Obtiene los envíos actualmente en un almacén
        /// </summary>
        Task<IEnumerable<ShipmentWarehouse>> GetCurrentShipmentsInWarehouseAsync(int warehouseId);

        /// <summary>
        /// Obtiene la ubicación actual de un envío
        /// </summary>
        Task<ShipmentWarehouse?> GetCurrentLocationAsync(int shipmentId);

        /// <summary>
        /// Verifica si un envío está en algún almacén
        /// </summary>
        Task<bool> IsShipmentInWarehouseAsync(int shipmentId);

        /// <summary>
        /// Obtiene lista paginada con filtros
        /// </summary>
        Task<ResponseData> GetAllAsync(ShipmentWarehouseQueryFilter filters);

        /// <summary>
        /// Obtiene un registro por ID
        /// </summary>
        Task<ShipmentWarehouse> GetByIdAsync(int id);

        /// <summary>
        /// Elimina un registro
        /// </summary>
        Task DeleteAsync(int id);
    }
}