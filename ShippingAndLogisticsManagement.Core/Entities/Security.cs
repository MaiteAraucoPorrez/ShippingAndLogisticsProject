using ShippingAndLogisticsManagement.Core.Enum;

namespace ShippingAndLogisticsManagement.Core.Entities
{
    public partial class Security: BaseEntity
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }

        public RoleType Role { get; set; }
    }
}
