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

        public async Task<DailySummary> GetDailySummary(int userId, string dateString)
        {
            var date = DateTime.Parse(dateString);
            var total = GetAllMoney(userId, date,date);
            var percent = await GetPercent(userId, date, date, total);
            var categories = GetAllCategoriesCount(userId, date, date);
            var dailySummary = new DailySummary()
            {
                Date = dateString,
                PercentPlan = percent,
                TotalCost = total
            };
            return dailySummary;
        }

        private async Task<double> GetPercent(int userId, DateTime startDate, DateTime endDate, double total)
        {
            double percent;
            var amount = await _dbContext.Plans.Where(p => p.UserID == userId && p.StartDate <= startDate && p.EndDate >= endDate).Select(p=>p.Amount).DefaultIfEmpty(-1).FirstAsync();
            if (amount == -1)
            {
                return amount;
            }
            percent = Math.Round(100 * total / amount, 1);

            return percent;
        }

        private double GetAllMoney(int userId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Include(r => r.ReceiptProducts)
                .Select(r=>r.ReceiptProducts
                    .Select(rp=>rp.Amount*rp.Price)
                    .Sum())
                .Sum();
        }

        private double GetAllShopsCount(int userId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Select(r => r.ShopID)
                .Sum();
        }

        private double GetAllProductsCount(int userId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Include(r => r.ReceiptProducts)
                .Select(r => r.ReceiptProducts
                    .Select(rp=>rp.ProductID)
                    .Sum())
                .Sum();
        }

        private double GetAllCategoriesCount(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var caetgories = _dbContext.Receipts
                    .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                    .Include(r => r.ReceiptProducts)
                    .ThenInclude(rp => rp.Product)
                    .ThenInclude(p => p.ProductCategories)
                    .SelectMany(x => x.ReceiptProducts
                        .SelectMany(rp => rp.Product.ProductCategories
                            .Select(pc => pc.CategoryID))
                        .Distinct()
                        .ToList());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Include(r => r.ReceiptProducts)
                .ThenInclude(rp=>rp.Product)
                .ThenInclude(p=>p.ProductCategories)
                .Select(r => r.ReceiptProducts
                    .Select(rp => rp.Product.ProductCategories
                        .Select(pc=>pc.CategoryID))
                    .Distinct())
                .Distinct()
                .Count();
        }
    }

    public interface ISummaryHelper
    {
        Task<DailySummary> GetDailySummary(int userId, string date);
    }
}
