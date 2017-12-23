using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeWallet_API.Models.PUT
{
    public class PlanEdit
    {
        public int ID { get; set; }
        public double Amount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int UserID { get; set; }
    }
}
