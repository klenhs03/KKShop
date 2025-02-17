using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKShop.DomainModels
{
    public class Shipper
    {
        public int ShipperID { get; set; }
        public string ShipperName { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;

    }
}
