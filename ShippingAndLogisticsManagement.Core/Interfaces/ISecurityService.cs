using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Defines methods for authenticating users and managing user registration within the security system.
    /// </summary>
    /// <remarks>Implementations of this interface should provide mechanisms for verifying user credentials
    /// and registering new users. Methods may perform asynchronous operations, such as database access or external
    /// service calls.</remarks>
    public interface ISecurityService
    {
        /// <summary>
        /// Retrieves the security information for a user based on the provided login credentials.
        /// </summary>
        /// <param name="login">The user login credentials used to authenticate and identify the user. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the security information for the
        /// authenticated user, or null if the credentials are invalid.</returns>
        Task<Security> GetLoginByCredentials(UserLogin login);


        /// <summary>
        /// Registers a new user using the specified security credentials.
        /// </summary>
        /// <param name="security">The security credentials and user information required for registration. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous registration operation.</returns>
        Task RegisterUser(Security security);
    }
}
