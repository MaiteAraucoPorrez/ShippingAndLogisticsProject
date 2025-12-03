namespace ShippingAndLogisticsManagement.Core.Enum
{
    /// <summary>
    /// Specifies the supported database providers for configuration and connection purposes.
    /// </summary>
    /// <remarks>Use this enumeration to indicate which database system the application should connect to. The
    /// selected provider determines the connection string format and the underlying data access
    /// implementation.</remarks>
    public enum DatabaseProvider
    {
        SqlServer,
        MySql
    }
}
