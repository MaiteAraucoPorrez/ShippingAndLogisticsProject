using ShippingAndLogisticsManagement.Core.Entities;
using System.Data;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Unit of Work pattern to manage transactions and repositories
    /// </summary>
    public interface IUnitOfWork: IDisposable
    {
        ICustomerRepository CustomerRepository { get; }
        IPackageRepository PackageRepository { get; }
        IRouteRepository RouteRepository { get; }
        IShipmentRepository ShipmentRepository { get; }
        ISecurityRepository SecurityRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
        Task BeginTransaccionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        IDbConnection? GetDbConnection();
        IDbTransaction? GetDbTransaction();
    }
}
