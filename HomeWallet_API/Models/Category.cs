using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
