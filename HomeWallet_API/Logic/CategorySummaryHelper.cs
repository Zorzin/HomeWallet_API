using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class CategorySummaryHelper : ICategorySummaryHelper
    {
        private readonly DBContext _dbContext;
        private readonly IDbHelper _dbHelper;

        public CategorySummaryHelper(DBContext dbContext, IDbHelper dbHelper)
        {
            _dbContext = dbContext;
            _dbHelper = dbHelper;
        }

        public async Task<CategorySummary> GetCategorySummary(int userId, int categoryId, string startDate, string endDate)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            return new CategorySummary()
            {
                ProductsAmount = await GetProductsCountForCategory(userId, categoryId, start, end),
                MoneySpent = await GetMoneySpentOnCategory(userId, categoryId, start, end),
                MoneySpentOnCategoryPerDay = await GetAvarageMoneySpentOnCategoryPerDay(userId, categoryId, start, end),
                MostPopularShop = await GetMostPopularShopForCategory(userId, categoryId, start, end),
                MoneySpentInShops = await GetMoneySpentInShops(userId, categoryId, start, end)
            };
        }

        private async Task<List<ChartData>> GetMoneySpentInShops(int userId, int categoryId, DateTime start, DateTime end)
        {
            var moneySpentInShops = new List<ChartData>();
            var shops = await GetAllShops(userId, categoryId, start, end);
            foreach (var shop in shops)
            {
                moneySpentInShops.Add(new ChartData()
                {
                    Name = _dbHelper.GetShopName(shop),
                    Value = await GetMoneySpentOnCategoryInShop(userId, categoryId, shop, start, end)
                });
            }
            return moneySpentInShops;
        }

        private async Task<double> GetAvarageMoneySpentOnCategoryPerDay(int userId, int categoryId, DateTime startDate,
            DateTime endDate)
        {
            var total = await GetMoneySpentOnCategory(userId, categoryId, startDate, endDate);
            return Math.Round(total / GetDaysBetweenDates(startDate, endDate), 2);
        }

        private int GetDaysBetweenDates(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).Days +1;
        }

        private async Task<int> GetProductsCountForCategory(int userId, int categoryId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ProductCategories
                .Include(pc => pc.Category)
                .Include(pc => pc.Product)
                .ThenInclude(p => p.ReceiptProducts)
                .ThenInclude(rp => rp.Receipt)
                .CountAsync(pc => pc.CategoryID == categoryId && pc.Category.UserID == userId && pc.Product.ReceiptProducts
                                 .Any(rp => rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate));
        }

        private async Task<double> GetMoneySpentOnCategory(int userId, int categoryId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Include(rp => rp.Product)
                .ThenInclude(p => p.ProductCategories)
                .Where(rp =>
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate &&
                    rp.Receipt.UserID == userId && rp.Product.ProductCategories.Any(pc => pc.CategoryID == categoryId))
                .Select(rp => rp.Price * rp.Amount)
                .SumAsync(),2);
        }

        private async Task<int> GetMostPopularShopForCategory(int userId, int categoryId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Receipts
                .Include(r => r.ReceiptProducts)
                .ThenInclude(rp => rp.Product)
                .ThenInclude(p => p.ProductCategories)
                .Where(r =>
                    r.PurchaseDate >= startDate && r.PurchaseDate <= endDate &&
                    r.UserID == userId && r.ReceiptProducts
                        .Any(rp => rp.Product.ProductCategories
                            .Any(pc => pc.CategoryID == categoryId)))
                .Select(r => r.ShopID)
                .GroupBy(s=>s)
                .OrderByDescending(x=>x.Count())
                .Select(x=>x.Key)
                .FirstAsync();
        }

        private async Task<List<int>> GetAllShops(int userId, int categoryId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Receipts
                .Include(r => r.ReceiptProducts)
                .ThenInclude(rp => rp.Product)
                .ThenInclude(p => p.ProductCategories)
                .Where(r =>
                    r.PurchaseDate >= startDate && r.PurchaseDate <= endDate &&
                    r.UserID == userId && r.ReceiptProducts
                        .Any(rp => rp.Product.ProductCategories
                            .Any(pc => pc.CategoryID == categoryId)))
                .Select(r => r.ShopID)
                .Distinct()
                .ToListAsync();
        }

        private async Task<double> GetMoneySpentOnCategoryInShop(int userId, int categoryId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Include(rp => rp.Product)
                .ThenInclude(p => p.ProductCategories)
                .Where(rp =>
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate &&
                    rp.Receipt.UserID == userId && rp.Receipt.ShopID == shopId &&
                    rp.Product.ProductCategories.Any(pc => pc.CategoryID == categoryId))
                .Select(rp => rp.Amount * rp.Price)
                .SumAsync(),2);
        }
    }

    public interface ICategorySummaryHelper
    {
        Task<CategorySummary> GetCategorySummary(int userId, int categoryId, string startDate, string endDate);
    }
}
