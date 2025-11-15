using Microsoft.EntityFrameworkCore;
using ShippingAndLogisticsManagement.Core.Entities;
using ShippingAndLogisticsManagement.Core.Interfaces;
using ShippingAndLogisticsManagement.Infrastructure.Data;

namespace ShippingAndLogisticsManagement.Infrastructure.Repositories
{
    public class SecurityRepository : BaseRepository<Security>, ISecurityRepository
    {
        private readonly IDapperContext _dapper;
        public SecurityRepository(LogisticContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }
        public async Task<Security> GetLoginByCredentials(UserLogin login)
        {
            return await _entities.FirstOrDefaultAsync(x => x.Login == login.User
            && x.Password == login.Password);
        }
    }

}


