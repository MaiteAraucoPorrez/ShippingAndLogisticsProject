using ShippingAndLogisticsManagement.Core.Enum;
using System.Data;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface IDbConnectionFactory
    {
        DatabaseProvider Provider { get; }
        IDbConnection CreateConnection();
    }
}
