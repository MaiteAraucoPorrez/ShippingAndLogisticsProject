using ShippingAndLogisticsManagement.Core.Enum;

namespace ShippingAndLogisticsManagement.Core.Entities
{
    /// <summary>
    /// Represents a user security entity containing authentication and authorization information.
    /// </summary>
    /// <remarks>This class encapsulates user credentials and role assignment for access control purposes. It
    /// is typically used to manage user accounts and permissions within an application.</remarks>
    public partial class Security: BaseEntity
    {
        /// <summary>
        /// Gets or sets the login name used to identify the user.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the password used for authentication.
        /// </summary>
        /// <remarks>The password should be stored and transmitted securely to prevent unauthorized
        /// access. Avoid logging or exposing the password in application output.</remarks>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the role assigned to the user.
        /// </summary>
        public RoleType Role { get; set; }
    }
}
