using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class CategorySummary
    {
        public int ProductsAmount { get; set; }
        public double MoneySpent { get; set; }
        public int MostPopularShop { get; set; }
        public List<ChartData> MoneySpentInShops { get; set; }
        public double MoneySpentOnCategoryPerDay { get; set; }
    }
}
