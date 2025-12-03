using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Defines a contract for accessing and managing security-related data, including authentication operations.
    /// </summary>
    /// <remarks>This interface extends <see cref="IBaseRepository{Security}"/> to provide additional methods
    /// specific to security and user authentication. Implementations are expected to support asynchronous operations
    /// for retrieving security entities based on user credentials.</remarks>
    public interface ISecurityRepository: IBaseRepository<Security>
    {
        /// <summary>
        /// Authenticates a user by validating the provided login credentials and retrieves the associated security
        /// information.
        /// </summary>
        /// <param name="userLogin">An object containing the user's login credentials. Must not be null and should include valid username and
        /// password values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Security"/> object
        /// with the user's security details if authentication succeeds; otherwise, the result is null.</returns>
        Task<Security> GetLoginByCredentials(UserLogin userLogin);
    }
}