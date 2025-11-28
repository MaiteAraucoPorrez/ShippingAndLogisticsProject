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
        public readonly IAddressRepository? _addressRepository;
        public readonly ICustomerRepository? _customerRepository;
        public readonly IPackageRepository? _packageRepository;
        public readonly IRouteRepository? _routeRepository;
        public readonly IShipmentRepository? _shipmentRepository;
        public readonly IShipmentWarehouseRepository? _shipwarehouseRepository;
        public readonly IWarehouseRepository? _warehouseRepository;
        public readonly IDriverRepository? _driverRepository;
        public readonly IVehicleRepository? _vehicleRepository;
        public readonly ISecurityRepository? _securityRepository;
        public readonly IDapperContext _dapper;

        private IDbContextTransaction? _efTransaction;
        public UnitOfWork(LogisticContext context, IDapperContext dapper)
        {
            _context = context;
            _dapper = dapper;

        }

        public IAddressRepository AddressRepository =>
           _addressRepository ?? new AddressRepository(_context, _dapper);
        public IShipmentRepository ShipmentRepository =>
            _shipmentRepository ?? new ShipmentRepository(_context, _dapper);

        public IPackageRepository PackageRepository =>
            _packageRepository ?? new PackageRepository(_context, _dapper);

        public ICustomerRepository CustomerRepository =>
            _customerRepository ?? new CustomerRepository(_context, _dapper);

        public IRouteRepository RouteRepository =>
            _routeRepository ?? new RouteRepository(_context, _dapper);

        public IShipmentWarehouseRepository ShipmentWarehouseRepository =>
            _shipwarehouseRepository ?? new ShipmentWarehouseRepository(_context, _dapper);

        public IWarehouseRepository WarehouseRepository =>
            _warehouseRepository ?? new WarehouseRepository(_context, _dapper);

        public IDriverRepository DriverRepository =>
            _driverRepository ?? new DriverRepository(_context, _dapper);

        public IVehicleRepository VehicleRepository =>
            _vehicleRepository ?? new VehicleRepository(_context, _dapper);

        public ISecurityRepository SecurityRepository =>
            _securityRepository ?? new SecurityRepository(_context, _dapper);



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
