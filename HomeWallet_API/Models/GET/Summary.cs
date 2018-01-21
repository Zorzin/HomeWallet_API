using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models.GET
{
    public class Summary
    {
        public double TotalCost { get; set; }
        public double PercentPlan { get; set; }
        public double MinProductCost { get; set; }
        public double MaxProductCost { get; set; }
        public double AverageProductCost { get; set; }
        public double AverageReceiptCost { get; set; }
        public double ShopsCount { get; set; }
        public double ProductsCount { get; set; }
        public double CategoriesCount { get; set; }
        public List<ChartData> EachCategoryCost { get; set; }
        public List<ChartData> EachShopMoney { get; set; }
        public List<ChartData> EachShopProducts { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
