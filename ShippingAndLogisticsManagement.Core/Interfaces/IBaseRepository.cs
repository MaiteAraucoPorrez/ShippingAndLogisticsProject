using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}
