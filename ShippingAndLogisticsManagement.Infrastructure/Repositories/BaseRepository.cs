using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.Data;

namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class BaseRepository<T>
       : IBaseRepository<T> where T : BaseEntity
    {
        private readonly LogisticContext _context;
        protected readonly DbSet<T> _entities;
        public BaseRepository(LogisticContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            _entities.Add(entity);
            //await _context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _entities.Update(entity);
            //await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            T entity = await GetById(id);
            _entities.Remove(entity);
            //await _context.SaveChangesAsync();
        }

    }
}
