using ShippingAndLogisticsManagement.Core.Entities;
using System.Data;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Unit of Work pattern to manage transactions and repositories
    /// </summary>
    public interface IUnitOfWork: IDisposable
    {
        IShipmentRepository ShipmentRepository { get; }
        IPackageRepository PackageRepository { get; }
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
