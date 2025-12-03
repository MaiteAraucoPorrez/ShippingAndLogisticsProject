namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    /// <summary>
    /// Defines methods for securely hashing passwords and verifying password hashes.
    /// </summary>
    /// <remarks>Implementations of this interface should use cryptographically secure algorithms to protect
    /// password data. The interface enables applications to store password hashes and validate user credentials without
    /// exposing plain-text passwords.</remarks>
    public interface IPasswordService
    {
        /// <summary>
        /// Generates a secure hash for the specified password.
        /// </summary>
        /// <param name="password">The password to be hashed. Cannot be null or empty.</param>
        /// <returns>A string containing the hashed representation of the password.</returns>
        string Hash(string password);

        /// <summary>
        /// Verifies whether the specified password matches the provided password hash.
        /// </summary>
        /// <param name="hash">The hashed password to compare against. Cannot be null or empty.</param>
        /// <param name="password">The plain-text password to verify. Cannot be null or empty.</param>
        /// <returns>true if the password matches the hash; otherwise, false.</returns>
        bool VerifyPassword(string hash, string password);
    }
}
