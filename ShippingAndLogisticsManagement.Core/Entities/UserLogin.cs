namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Represents user credentials required for authentication, including a username and password.
    /// </summary>
    /// <remarks>Use this class to encapsulate login information when authenticating users in an application.
    /// The properties should be populated with valid credential values before submitting for authentication. This class
    /// does not perform validation or encryption; ensure sensitive data is handled securely.</remarks>
    public class UserLogin
    {
        /// <summary>
        /// Gets or sets the username associated with the current context.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the password used for authentication.
        /// </summary>
        public string Password { get; set; }
    }
}
