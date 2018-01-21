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
            var dailySummary = new DailySummary()
            {
                Date = dateString,
                PercentPlan = percent,
                TotalCost = total
            };
            return dailySummary;
        }

        private int GetProductAmountBoughtInShop(int userId, DateTime startDate, DateTime endDate, int shopId)
        {
            return _dbContext.ReceiptProducts
                .Include(r => r.Receipt)
                .Where(r => r.Receipt.PurchaseDate >= startDate && r.Receipt.PurchaseDate <= endDate &&
                            r.Receipt.UserID == userId && r.Receipt.ShopID == shopId)
                .Select(r => r.ProductID)
                .Count();
        }

        private double GetMoneySpentOnCategory(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            return _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Include(p => p.Product)
                .ThenInclude(pc => pc.ProductCategories)
                .Where(rp =>
                    rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate && rp.Product.ProductCategories.Any(pc=>pc.CategoryID==categoryId))
                .Select(rp => rp.Amount * rp.Price)
                .Sum();
        }

        private double GetMoneySpentOnProduct(int userId, DateTime startDate, DateTime endDate, int productId)
        {
            return _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Include(r => r.ReceiptProducts)
                .Select(r => r.ReceiptProducts
                    .Where(rp => rp.ProductID==productId)
                    .Select(rp => rp.Amount * rp.Price)
                    .Sum())
                .Sum();
        }

        private double GetMoneySpentInShop(int userId, DateTime startDate, DateTime endDate, int shopId)
        {
            return _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId && r.ShopID == shopId)
                .Include(r=>r.ReceiptProducts)
                .Select(r => r.ReceiptProducts
                    .Select(rp=>rp.Amount * rp.Price)
                    .Sum())
                .Sum();
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

        private int GetAllShopsCount(int userId, DateTime startDate, DateTime endDate)
        {
            return GetAllShops(userId,startDate,endDate).Count;
        }

        private List<int> GetAllShops(int userId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Select(r => r.ShopID)
                .Distinct()
                .ToList();
        }

        private int GetAllProductsCount(int userId, DateTime startDate, DateTime endDate)
        {
            return GetAllProducts(userId, startDate, endDate).Count;
        }

        private List<int> GetAllProducts(int userId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Include(r => r.ReceiptProducts)
                .Select(r => r.ReceiptProducts
                    .Select(rp=>rp.ProductID)
                    .Distinct()
                    .ToList())
                .Distinct()
                .SelectMany(x=>x)
                .ToList();
        }

        private double GetAllCategoriesCount(int userId, DateTime startDate, DateTime endDate)
        {
            return GetAllCategories(userId, startDate, endDate).Count;
        }

        private List<int> GetAllCategories(int userId, DateTime startDate, DateTime endDate)
        {
            var receiptList = _dbContext.Receipts
                            .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                            .Include(r => r.ReceiptProducts)
                            .ThenInclude(rp => rp.Product)
                            .ThenInclude(p => p.ProductCategories)
                            .Select(x => x.ReceiptProducts
                                .SelectMany(rp => rp.Product.ProductCategories
                                    .Select(pc => pc.CategoryID))
                                .Distinct()
                                .ToList())
                            .ToList();

            var categories = new List<int>();

            foreach (var receipt in receiptList)
            {
                foreach (var category in receipt)
                {
                    if (!categories.Contains(category))
                    {
                        categories.Add(category);
                    }
                }
            }

            return categories;
        }
    }

    public interface ISummaryHelper
    {
        Task<DailySummary> GetDailySummary(int userId, string date);
    }
}
