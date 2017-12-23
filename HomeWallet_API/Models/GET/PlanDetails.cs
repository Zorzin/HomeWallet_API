using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models.GET
{
    public class PlanDetails : Plan
    {
        public double MoneyLeft { get; set; }
        public double AlreadySpent { get; set; }
        public double DaysLeft { get; set; }
        public double DaysBehind { get; set; }
        public double AverageLeft { get; set; }
        public double AlreadySpentPercent { get; set; }
    }
}
