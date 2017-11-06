using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class ProductCategory
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int ProductID { get; set; }

        public Product Product { get; set; }
        public Category Category { get; set; }
    }
}
