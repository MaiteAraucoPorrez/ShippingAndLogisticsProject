using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IRouteRepository
    {
        Task<Route> GetByIdAsync(int id);
    }
}
