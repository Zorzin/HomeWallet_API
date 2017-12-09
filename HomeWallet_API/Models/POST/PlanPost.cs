using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models.POST
{
    public class PlanPost
    {
        public int Amount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
