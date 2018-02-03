using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class ProductSummary
    {
        public int TotalTimesBoughtAmount { get; set; }
        public double TotalMoneySpent { get; set; }
        public double MaxPrice { get; set; }
        public double MinPrice { get; set; }
        public double AveragePrice { get; set; }
        public int ShopsAmount { get; set; }
        public List<ChartData> AveragePriceInShops { get; set; }
        public int ShopWithCheapestPrice { get; set; }
        public string ShopWithCheapestPriceName { get; set; }
        public int ShopWithMostExpensivePrice { get; set; }
        public string ShopWithMostExpensivePriceName { get; set; }
        public double AverageAmountOnReceipt { get; set; }
        public double MinAmountOnReceipt { get; set; }
        public double MaxAmountOnReceipt { get; set; }
        public double PercentageAmountOfTotalMoneySpent { get; set; }
        public double AverageCostPerDay { get; set; }
    }
}
