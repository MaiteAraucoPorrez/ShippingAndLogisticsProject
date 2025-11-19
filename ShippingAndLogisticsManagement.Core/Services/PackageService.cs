using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(PackageQueryFilter packageQueryFilter)
        {
            if (packageQueryFilter == null)
                throw new ArgumentNullException(nameof(packageQueryFilter));

            // Basic validations
            if (packageQueryFilter.PageNumber <= 0) packageQueryFilter.PageNumber = 1;
            if (packageQueryFilter.PageSize <= 0) packageQueryFilter.PageSize = 10;

            var packages = await _unitOfWork.PackageRepository.GetAllDapperAsync(packageQueryFilter);

            // Applying filters
            if (packageQueryFilter.ShipmentId.HasValue)
                packages = packages.Where(p => p.ShipmentId == packageQueryFilter.ShipmentId.Value);

            if (!string.IsNullOrWhiteSpace(packageQueryFilter.Description))
                packages = packages.Where(p => p.Description.Contains(packageQueryFilter.Description, StringComparison.OrdinalIgnoreCase));

            if (packageQueryFilter.MinWeight.HasValue)
                packages = packages.Where(p => p.Weight >= packageQueryFilter.MinWeight.Value);

            if (packageQueryFilter.MaxWeight.HasValue)
                packages = packages.Where(p => p.Weight <= packageQueryFilter.MaxWeight.Value);

            if (packageQueryFilter.MinPrice.HasValue)
                packages = packages.Where(p => p.Price >= packageQueryFilter.MinPrice.Value);

            if (packageQueryFilter.MaxPrice.HasValue)
                packages = packages.Where(p => p.Price <= packageQueryFilter.MaxPrice.Value);

            // Creating PagedList<Package>
            var pageNumber = packageQueryFilter.PageNumber;
            var pageSize = packageQueryFilter.PageSize;
            var sourceList = packages.ToList();
            var count = sourceList.Count;
            var items = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Building PagedList<object> for ResponseData
            var itemsAsObject = items.Cast<object>().ToList();
            var paged = new PagedList<object>(itemsAsObject, count, pageNumber, pageSize);

            var messages = new Message[] { new() { Type = "Information", Description = "Paquetes recuperados correctamente" } };

            return new ResponseData()
            {
                Messages = messages,
                Pagination = paged,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<PackageSummaryResponse> GetPackageSummaryAsync(int shipmentId)
        {
            if (shipmentId <= 0)
                throw new ArgumentException("El ID del envío debe ser mayor a 0", nameof(shipmentId));

            var shipment = await _unitOfWork.ShipmentRepository.GetById(shipmentId);
            if (shipment == null)
                throw new KeyNotFoundException($"El envío con ID {shipmentId} no existe");

            var summary = await _unitOfWork.PackageRepository.GetPackageSummaryAsync(shipmentId);

            if (summary == null)
            {
                return new PackageSummaryResponse
                {
                    ShipmentId = shipmentId,
                    TotalPackages = 0,
                    TotalWeight = 0,
                    TotalValue = 0,
                    AvgWeight = 0,
                    AvgValue = 0
                };
            }

            return summary;
        }

        public async Task<IEnumerable<PackageDetailResponse>> GetPackageDetailsAsync()
        {
            var details = await _unitOfWork.PackageRepository.GetPackageWithDetailsAsync();

            if (details == null || !details.Any())
                throw new KeyNotFoundException("No se encontraron paquetes con detalles");

            return details;
        }

        public async Task<IEnumerable<Package>> GetHeavyPackagesAsync(double minWeight)
        {
            if (minWeight <= 0)
                throw new BusinessException("El peso mínimo debe ser mayor a 0");

            var packages = await _unitOfWork.PackageRepository.GetHeavyPackagesAsync(minWeight);

            if (packages == null || !packages.Any())
                throw new KeyNotFoundException($"No se encontraron paquetes con peso mayor a {minWeight} kg");

            return packages;
        }

        public async Task<IEnumerable<Package>> GetAllDapperAsync()
        {
            var packages = await _unitOfWork.PackageRepository.GetRecentPackagesAsync(5);

            return packages;
        }

        public async Task<Package> GetByIdDapperAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del paquete debe ser mayor a 0");

            var package = await _unitOfWork.PackageRepository.GetByIdDapperAsync(id);
            return package;
        }

        public async Task<Package> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID del paquete debe ser mayor a 0");

            var package = await _unitOfWork.PackageRepository.GetById(id);
            return package;
        }

        public async Task InsertAsync(Package package)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));

            // Business validations
            if (string.IsNullOrWhiteSpace(package.Description) || package.Description.Length < 3)
                throw new BusinessException("La descripcion es invalida");

            if (package.Weight <= 0 || package.Weight > 100)
                throw new BusinessException("Peso invalido. Debe ser mayor que 0 y menor o igual a 100 kg");

            if (package.Price <= 0)
                throw new BusinessException("El precio debe ser mayor que 0");

            // Shipment existe y esta en estado permitido
            var shipment = await _unitOfWork.ShipmentRepository.GetById(package.ShipmentId);
            if (shipment == null) throw new BusinessException("El envio asociado no existe");

            if (shipment.State == "Delivered")
                throw new BusinessException("No se pueden agregar paquetes a un envio entregado");

            // Package limit per shipment: 50
            var existingPackages = await _unitOfWork.PackageRepository.GetByShipmentIdDapperAsync(package.ShipmentId);
            if (existingPackages != null && existingPackages.Count() >= 50)
                throw new BusinessException("El envio alcanzó el numero máximo de paquetes permitidos");

            await _unitOfWork.PackageRepository.Add(package);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Package package)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));

            var shipment = await _unitOfWork.ShipmentRepository.GetById(package.ShipmentId);
            if (shipment != null && shipment.State == "Delivered")
                throw new BusinessException("No se puede modificar un paquete de un envío entregado");

            // Validations
            if (string.IsNullOrWhiteSpace(package.Description) || package.Description.Length < 3)
                throw new BusinessException("La descripcion es invalida");

            if (package.Weight <= 0 || package.Weight > 100)
                throw new BusinessException("Peso invalido. Debe ser mayor que 0 y menor o igual a 100 kg");

            if (package.Price <= 0)
                throw new BusinessException("El precio debe ser mayor que 0");

            await _unitOfWork.PackageRepository.Update(package);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.PackageRepository.GetById(id);
            if (existing == null) throw new KeyNotFoundException("El paquete no existe");

            var shipment = await _unitOfWork.ShipmentRepository.GetById(existing.ShipmentId);
            if (shipment != null && shipment.State == "Delivered")
                throw new BusinessException("No se puede eliminar un paquete de un envio entregado");

            await _unitOfWork.PackageRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}