using ShippingAndLogisticsManagement.Core.Entities;
using System.Data;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        IShipmentRepository ShipmentRepository { get; }
        IBaseRepository<Customer> CustomerRepository { get; }
        IBaseRepository<Route> RouteRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();

        Task BeginTransaccionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        IDbConnection? GetDbConnection();
        IDbTransaction? GetDbTransaction();
    }
}
