using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class ReceiptPost
    {
        public int ShopId { get; set; }
        public string Date { get; set; }
        public ReceiptProductPost[] Products { get; set; }

    }

    public class ReceiptProductPost
    {
        public int ProductId { get; set; }
        public Double Amount { get; set; }
        public Double Price { get; set; }
    }
}
