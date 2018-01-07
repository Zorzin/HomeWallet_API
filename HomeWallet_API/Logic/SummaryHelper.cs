using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using HomeWallet_API.Models.GET;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class SummaryHelper : ISummaryHelper
    {
        private DBContext _dbContext;

        public SummaryHelper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DailySummary GetDailySummary(int userId, string dateString)
        {
            var date = DateTime.Parse(dateString);
            var total = GetAllMoney(userId, date);
            var percent = GetPercent(userId, date, total);
            var dailySummary = new DailySummary()
            {
                Date = dateString,
                PercentPlan = percent,
                TotalCost = total
            };
            return dailySummary;
        }

        private double GetPercent(int userId, DateTime date, double total)
        {
            double percent;
            var plan = _dbContext.Plans.FirstOrDefault(p => p.UserID == userId && p.StartDate <= date && p.EndDate >= date);
            if (plan != null)
            {
                percent = Math.Round(100 * total / plan.Amount, 1);
            }
            else
            {
                percent = -1;
            }

            return percent;
        }

        private double GetAllMoney(int userId, DateTime date)
        {
            var receipts = _dbContext.Receipts.Where(r => r.PurchaseDate == date && r.UserID == userId).Include(r => r.ReceiptProducts).ToList();
            double result = 0;

            foreach (var receipt in receipts)
            {
                foreach (var productreceipt in receipt.ReceiptProducts)
                {
                    result += productreceipt.Amount * productreceipt.Price;
                }
            }
            return result;
        }
    }

    public interface ISummaryHelper
    {
        DailySummary GetDailySummary(int userId, string date);
    }
}
