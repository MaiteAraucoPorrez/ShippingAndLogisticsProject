using ShippingAndLogisticsManagement.Core.CustomEntities;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Exceptions;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Core.QueryFilters;
using System.Net;

namespace ShippingAndLogisticsManagement.Core.Services
{
    public class RouteService : IRouteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RouteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseData> GetAllAsync(RouteQueryFilter routeQueryFilter)
        {
            if (routeQueryFilter == null)
                throw new ArgumentNullException(nameof(routeQueryFilter));

            // Basic validations
            if (routeQueryFilter.PageNumber <= 0) routeQueryFilter.PageNumber = 1;
            if (routeQueryFilter.PageSize <= 0) routeQueryFilter.PageSize = 10;

            var routes = await _unitOfWork.RouteRepository.GetAllDapperAsync(routeQueryFilter);

            // Applying filters
            if (!string.IsNullOrWhiteSpace(routeQueryFilter.Origin))
                routes = routes.Where(r => r.Origin.Contains(routeQueryFilter.Origin, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(routeQueryFilter.Destination))
                routes = routes.Where(r => r.Destination.Contains(routeQueryFilter.Destination, StringComparison.OrdinalIgnoreCase));

            if (routeQueryFilter.MinDistance.HasValue)
                routes = routes.Where(r => r.DistanceKm >= routeQueryFilter.MinDistance.Value);

            if (routeQueryFilter.MaxDistance.HasValue)
                routes = routes.Where(r => r.DistanceKm <= routeQueryFilter.MaxDistance.Value);

            if (routeQueryFilter.MinCost.HasValue)
                routes = routes.Where(r => r.BaseCost >= routeQueryFilter.MinCost.Value);

            if (routeQueryFilter.MaxCost.HasValue)
                routes = routes.Where(r => r.BaseCost <= routeQueryFilter.MaxCost.Value);

            if (routeQueryFilter.IsActive.HasValue)
                routes = routes.Where(r => r.IsActive == routeQueryFilter.IsActive.Value);

            // Creating PagedList
            var pageNumber = routeQueryFilter.PageNumber;
            var pageSize = routeQueryFilter.PageSize;
            var sourceList = routes.ToList();
            var count = sourceList.Count;
            var items = sourceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Building PagedList<object> for ResponseData
            var itemsAsObject = items.Cast<object>().ToList();
            var paged = new PagedList<object>(itemsAsObject, count, pageNumber, pageSize);

            var messages = new Message[] { new() { Type = "Information", Description = "Rutas recuperadas correctamente" } };

            return new ResponseData()
            {
                Messages = messages,
                Pagination = paged,
                StatusCode = HttpStatusCode.OK
            };
        }

        public async Task<IEnumerable<Route>> GetAllDapperAsync()
        {
            var routes = await _unitOfWork.RouteRepository.GetRecentRoutesAsync(10);
            return routes;
        }

        public async Task<IEnumerable<Route>> GetActiveRoutesAsync()
        {
            var routes = await _unitOfWork.RouteRepository.GetActiveRoutesAsync();
            return routes;
        }

        public async Task<IEnumerable<RouteRankingResponse>> GetMostUsedRoutesAsync(int limit = 10)
        {
            if (limit <= 0 || limit > 100)
                throw new ArgumentException("El límite debe estar entre 1 y 100");

            var ranking = await _unitOfWork.RouteRepository.GetMostUsedRoutesAsync(limit);
            return ranking;
        }

        public async Task<Route> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID de la ruta debe ser mayor a 0");

            var route = await _unitOfWork.RouteRepository.GetById(id);
            return route;
        }

        public async Task<Route> GetByIdDapperAsync(int id)
        {
            if (id <= 0)
                throw new BusinessException("El ID de la ruta debe ser mayor a 0");

            var route = await _unitOfWork.RouteRepository.GetByIdDapperAsync(id);
            return route;
        }

        public async Task InsertAsync(Route route)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));

            // Business validations
            if (string.IsNullOrWhiteSpace(route.Origin) || route.Origin.Length < 3 || route.Origin.Length > 100)
                throw new BusinessException("El origen debe tener entre 3 y 100 caracteres");

            if (string.IsNullOrWhiteSpace(route.Destination) || route.Destination.Length < 3 || route.Destination.Length > 100)
                throw new BusinessException("El destino debe tener entre 3 y 100 caracteres");

            // Origin and destination must be different
            if (route.Origin.Equals(route.Destination, StringComparison.OrdinalIgnoreCase))
                throw new BusinessException("El origen y destino deben ser diferentes");

            // Check for duplicate route
            var existingRoute = await _unitOfWork.RouteRepository.GetByOriginDestinationAsync(route.Origin, route.Destination);
            if (existingRoute != null)
                throw new BusinessException($"Ya existe una ruta de {route.Origin} a {route.Destination}");

            if (route.DistanceKm <= 0)
                throw new BusinessException("La distancia debe ser mayor a 0 km");

            if (route.BaseCost < 0)
                throw new BusinessException("El costo base no puede ser negativo");

            await _unitOfWork.RouteRepository.Add(route);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Route route)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));

            var existing = await _unitOfWork.RouteRepository.GetById(route.Id);
            if (existing == null) throw new KeyNotFoundException("La ruta no existe");

            // Validations
            if (string.IsNullOrWhiteSpace(route.Origin) || route.Origin.Length < 3 || route.Origin.Length > 100)
                throw new BusinessException("El origen debe tener entre 3 y 100 caracteres");

            if (string.IsNullOrWhiteSpace(route.Destination) || route.Destination.Length < 3 || route.Destination.Length > 100)
                throw new BusinessException("El destino debe tener entre 3 y 100 caracteres");

            if (route.Origin.Equals(route.Destination, StringComparison.OrdinalIgnoreCase))
                throw new BusinessException("El origen y destino deben ser diferentes");

            // Check if origin/destination changed
            if (!existing.Origin.Equals(route.Origin, StringComparison.OrdinalIgnoreCase) ||
                !existing.Destination.Equals(route.Destination, StringComparison.OrdinalIgnoreCase))
            {
                // If has shipments, cannot change origin/destination
                var hasShipments = await _unitOfWork.RouteRepository.HasShipmentsAsync(route.Id);
                if (hasShipments)
                    throw new BusinessException("No se puede modificar origen/destino de una ruta con envíos asociados");

                // Check duplicate with new origin/destination
                var duplicateRoute = await _unitOfWork.RouteRepository.GetByOriginDestinationAsync(route.Origin, route.Destination);
                if (duplicateRoute != null && duplicateRoute.Id != route.Id)
                    throw new BusinessException($"Ya existe otra ruta de {route.Origin} a {route.Destination}");
            }

            if (route.DistanceKm <= 0)
                throw new BusinessException("La distancia debe ser mayor a 0 km");

            if (route.BaseCost < 0)
                throw new BusinessException("El costo base no puede ser negativo");

            await _unitOfWork.RouteRepository.Update(route);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.RouteRepository.GetById(id);
            if (existing == null) throw new KeyNotFoundException("La ruta no existe");

            // Cannot delete route with shipments
            var hasShipments = await _unitOfWork.RouteRepository.HasShipmentsAsync(id);
            if (hasShipments)
                throw new BusinessException("No se puede eliminar una ruta con envíos asociados");

            await _unitOfWork.RouteRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
