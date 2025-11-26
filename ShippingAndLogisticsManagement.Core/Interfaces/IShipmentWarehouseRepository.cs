using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Define las operaciones del repositorio para la relación Shipment-Warehouse
    /// </summary>
    public interface IShipmentWarehouseRepository : IBaseRepository<ShipmentWarehouse>
    {
        /// <summary>
        /// Obtiene el historial de almacenes por los que pasó un envío
        /// </summary>
        Task<IEnumerable<ShipmentWarehouseHistoryResponse>> GetShipmentHistoryAsync(int shipmentId);

        /// <summary>
        /// Obtiene todos los envíos actualmente en un almacén
        /// </summary>
        Task<IEnumerable<ShipmentWarehouse>> GetCurrentShipmentsInWarehouseAsync(int warehouseId);

        /// <summary>
        /// Obtiene el registro activo (sin ExitDate) de un envío
        /// </summary>
        Task<ShipmentWarehouse> GetCurrentWarehouseForShipmentAsync(int shipmentId);

        /// <summary>
        /// Registra la entrada de un envío a un almacén
        /// </summary>
        Task RegisterEntryAsync(ShipmentWarehouse shipmentWarehouse);

        /// <summary>
        /// Registra la salida de un envío de un almacén
        /// </summary>
        Task RegisterExitAsync(int shipmentWarehouseId, DateTime exitDate, string dispatchedBy);

        /// <summary>
        /// Verifica si un envío está actualmente en un almacén
        /// </summary>
        Task<bool> IsShipmentInWarehouseAsync(int shipmentId);
    }
}