using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class Shop
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public ICollection<Receipt> Receipts { get; set; }
    }
}
