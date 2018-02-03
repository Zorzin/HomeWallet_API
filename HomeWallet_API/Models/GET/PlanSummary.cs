using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models
{
    public class PlanSummary
    {
        public double Amount { get; set; }
        public double MoneyLeft { get; set; }
        public double MoneyLeftPercentage { get; set; }
        public double MoneySpentPercentage { get; set; }
        public int MostPopularProduct { get; set; }
        public string MostPopularProductName { get; set; }
        public int MostExpensiveProduct { get; set; }
        public string MostExpensiveProductName { get; set; }
        public int MostPopularCategory { get; set; }
        public string MostPopularCategoryName { get; set; }
        public List<ChartData> MoneySpentOnCategories { get; set; }
        public double AverageMoneyLeftPerDay{ get; set; }
        public double ProductsBoughtAmount { get; set; }
        public double AverageMoneySpentPerDay { get; set; }
    }
}
