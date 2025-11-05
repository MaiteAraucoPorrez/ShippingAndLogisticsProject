using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.Data;
using System.Data;


namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LogisticContext _context;
        public readonly IShipmentRepository? _shipmentRepository;
        public readonly IPackageRepository? _packageRepository;
        public readonly IBaseRepository<Customer>? _customerRepository;
        public readonly IBaseRepository<Route>? _routeRepository;
        public readonly IDapperContext _dapper;

        private IDbContextTransaction? _efTransaction;
        public UnitOfWork(LogisticContext context, IDapperContext dapper)
        {
            _context = context;
            _dapper = dapper;

        }
        public IShipmentRepository ShipmentRepository =>
            _shipmentRepository ?? new ShipmentRepository(_context, _dapper);

        public IPackageRepository PackageRepository =>
            _packageRepository ?? new PackageRepository(_context, _dapper);

        public IBaseRepository<Customer> CustomerRepository =>
            _customerRepository ?? new BaseRepository<Customer>(_context);

        public IBaseRepository<Route> RouteRepository =>
            _routeRepository ?? new BaseRepository<Route>(_context);

        public void Dispose()
        {
            if (_context != null)
            {
                _efTransaction?.Dispose();
                _context.Dispose();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransaccionAsync()
        {
            if (_efTransaction == null)
            {
                _efTransaction = await _context.Database.BeginTransactionAsync();

                //Registrar coneccion/tx DapperContext
                var conn = _context.Database.GetDbConnection();
                var tx = _efTransaction.GetDbTransaction();
                _dapper.SetAmbientConnection(conn, tx);
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_efTransaction != null)
                {
                    await _efTransaction.CommitAsync();
                    _efTransaction.Dispose();
                    _efTransaction = null;
                }
            }
            finally
            {
                _dapper.ClearAmbientConnection();
            }
        }

        public async Task RollbackAsync()
        {
            if (_efTransaction != null)
            {
                await _efTransaction.RollbackAsync();
                _efTransaction.Dispose();
                _efTransaction = null;
            }

            _dapper.ClearAmbientConnection();
        }

        public IDbConnection? GetDbConnection()
        {
            //Retornar la coneccion subyacente del DbContext
            return _context.Database.GetDbConnection();
        }

        public IDbTransaction? GetDbTransaction()
        {
            return _efTransaction?.GetDbTransaction();
        }
    }
}
