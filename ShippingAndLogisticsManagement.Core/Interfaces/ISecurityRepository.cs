using ShippingAndLogisticsManagement.Core.Entities;

namespace ShippingAndLogisticsManagement.Core.Interfaces
{
    public interface ISecurityRepository: IBaseRepository<Security>
    {
        Task<Security> GetLoginByCredentials(UserLogin userLogin);
    }
}