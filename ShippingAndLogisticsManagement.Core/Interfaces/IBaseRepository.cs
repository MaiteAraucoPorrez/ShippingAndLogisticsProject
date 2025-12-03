using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Defines a generic repository interface for performing basic CRUD operations on entities of type <typeparamref
    /// name="T"/>.
    /// </summary>
    /// <remarks>This interface provides asynchronous methods for retrieving, adding, updating, and deleting
    /// entities. Implementations are expected to handle data persistence and may interact with various data sources
    /// such as databases or in-memory collections.</remarks>
    /// <typeparam name="T">The type of entity managed by the repository. Must inherit from <see cref="BaseEntity"/>.</typeparam>
    public interface IBaseRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Asynchronously retrieves all entities of type <typeparamref name="T"/> from the data source.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of all
        /// entities of type <typeparamref name="T"/>. If no entities are found, the collection will be empty.</returns>
        Task<IEnumerable<T>> GetAll();

        /// <summary>
        /// Asynchronously retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve. Must be greater than zero.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type T if found;
        /// otherwise, null.</returns>
        Task<T> GetById(int id);

        /// <summary>
        /// Asynchronously adds the specified entity to the data store.
        /// </summary>
        /// <param name="entity">The entity to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task Add(T entity);

        /// <summary>
        /// Asynchronously updates the specified entity in the data store.
        /// </summary>
        /// <param name="entity">The entity to update. Cannot be null. The entity must exist in the data store; otherwise, the update may
        /// fail.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task Update(T entity);

        /// <summary>
        /// Deletes the entity with the specified identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task Delete(int id);
    }
}
