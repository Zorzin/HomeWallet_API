using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models.PUT
{
    public class ReceiptEdit
    {
        public int ReceiptId { get; set; }
        public string Date { get; set; }
        public int ShopId { get; set; }
        public ReceiptProductEdit[] Products { get; set; }
    }

    public class ReceiptProductEdit
    {
        public int ReceiptProductId { get; set; }
        public int ProductId { get; set; }
        public Double Amount { get; set; }
        public Double Price { get; set; }
    }
}
