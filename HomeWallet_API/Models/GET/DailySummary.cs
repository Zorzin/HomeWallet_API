using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models.GET
{
    public class DailySummary
    {
        public double TotalCost { get; set; }
        public double PercentPlan { get; set; }
        public string Date { get; set; }
    }
}
