using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class Receipt
    {
        public int ID { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int ShopID { get; set; }
        public string UserID { get; set; }

        public ApplicationUser User { get; set; }
        public Shop Shop { get; set; }
        public ICollection<ReceiptProduct> ReceiptProducts { get; set; }
    }

    
}
