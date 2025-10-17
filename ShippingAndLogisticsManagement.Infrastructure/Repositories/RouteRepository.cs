using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.Data;


namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class RouteRepository: IRouteRepository
    {
        private readonly LogisticContext _context;
        public RouteRepository(LogisticContext context)
        {
            _context = context;
        }

        public async Task<Route> GetByIdAsync(int id)
        {
            var route = await _context.Routes.FirstOrDefaultAsync(x => x.Id == id);
            if (route == null)
                throw new InvalidOperationException($"No se encontró la ruta con Id {id}");
            return route;
        }
    }
}
