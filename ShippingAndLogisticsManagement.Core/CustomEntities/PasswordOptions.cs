using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingAndLogisticsManagement.Core.CustomEntities
{
    public class PasswordOptions
    {
        public int SaltSize { get; set; }
        public int KeySize { get; set; }
        public int Iterations { get; set; }
    }
}
