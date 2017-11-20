using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class ReceiptCyclicalPost
    {
        public int ShopId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Cycle { get; set; }
        public ReceiptProductPost[] Products { get; set; }
    }
}
