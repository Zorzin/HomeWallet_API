using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class ShopSummary
    {
        public int VisitAmount { get; set; }
        public double MoneySpentAmount { get; set; }
        public double MoneyAverageSpentAmount { get; set; }
        public double MoneyMaxSpentAmount { get; set; }
        public double MoneyMinSpentAmount { get; set; }
        public int ProductsBoughtTotalAmount { get; set; }
        public double ProductsBoughtAverageAmount { get; set; }
        public double ProductsBoughtMinAmount { get; set; }
        public double ProductsBoughtMaxAmount { get; set; }
        public int TotalCategoriesAmount { get; set; }
        public double AverageCategoryAmountForReceipt { get; set; }
        public double MinCategoryAmountForReceipt { get; set; }
        public double MaxCategoryAmountForReceipt { get; set; }
        public List<ChartData> AverageMoneySpentOnEachCategory { get; set; }
        public double AverageMoneySpentPerDay { get; set; }
        public double TotalProductAmountBought { get; set; }
        public double MinProductAmountBought { get; set; }
        public double MaxProductAmountBought { get; set; }
        public double AverageProductAmountBought { get; set; }
        public int MostPopularCategory { get; set; }
        public string MostPopularCategoryName { get; set; }
        public int LeastPopularCategory { get; set; }
        public string LeastPopularCategoryName { get; set; }
    }
}
