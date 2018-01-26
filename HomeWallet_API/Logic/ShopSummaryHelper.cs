using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class ShopSummaryHelper : IShopSummaryHelper
    {
        private readonly DBContext _dbContext;
        private readonly IDbHelper _dbHelper;

        public ShopSummaryHelper(DBContext dbContext, IDbHelper dbHelper)
        {
            _dbContext = dbContext;
            _dbHelper = dbHelper;
        }

        public async Task<ShopSummary> GetShopSummary(int userId, int shopId, string startDate, string endDate)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            var shopSummary = new ShopSummary()
            {
                AverageCategoryAmountForReceipt = await GetAverageAmountOfCategories(userId,shopId,start,end),
                AverageMoneySpentPerDay = await GetAverageMoneySpentInShopPerDay(userId,shopId,start,end),
                AverageMoneySpentOnEachCategory = await GetAverageMoneySpentOnCategories(userId,shopId,start,end),
                AverageProductAmountBought = await GetAverageProductsAmountBoughtInShop(userId,shopId,start,end),
                MaxCategoryAmountForReceipt = GetMaxAmountOfAllCategories(userId,shopId,start,end),
                MaxProductAmountBought = GetMaxProductsAmountBoughtInShop(userId,shopId,start,end),
                MinCategoryAmountForReceipt = GetMinAmountOfAllCategories(userId,shopId,start,end),
                MinProductAmountBought = GetMinProductsAmountBoughtInShop(userId,shopId,start,end),
                MoneyAverageSpentAmount = await GetAvarageMoneySpentInShop(userId,shopId,start,end),
                MoneyMaxSpentAmount = GetMaxMoneySpentInShop(userId,shopId,start,end),
                MoneyMinSpentAmount = GetMinMoneySpentInShop(userId,shopId,start,end),
                MoneySpentAmount = await GetTotalSpentAmount(userId,shopId,start,end),
                ProductsBoughtAverageAmount = await GetAverageProductsBoughtInShop(userId,shopId,start,end),
                ProductsBoughtMaxAmount = GetMaxProductsBoughtInShop(userId, shopId, start, end),
                ProductsBoughtMinAmount = GetMinProductsBoughtInShop(userId, shopId, start, end),
                ProductsBoughtTotalAmount = await GetTotalProductsBoughtInShop(userId,shopId,start,end),
                TotalCategoriesAmount = await GetAmountOfAllCategories(userId,shopId,start,end),
                TotalProductAmountBought = await GetTotalProductsAmountBoughtInShop(userId,shopId,start,end),
                VisitAmount = await GetVisitAmount(userId,shopId,start,end),
                MostPopularCategory = await GetMostPopularCategory(userId,shopId,start,end),
                LeastPopularCategory = await GetLeastPopularCategory(userId,shopId,start,end)
            };

            return shopSummary;
        }

        private async Task<int> GetVisitAmount(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Receipts
                .Where(r => r.ShopID == shopId && r.UserID == userId && r.PurchaseDate>= startDate && r.PurchaseDate <=endDate)
                .CountAsync();
        }

        private async Task<double> GetTotalSpentAmount(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.Price * rp.Amount)
                .SumAsync(), 2);
        }

        private double GetMaxMoneySpentInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(_dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => new {rp.ReceiptID, D = rp.Price * rp.Amount})
                .GroupBy(rp => rp.ReceiptID)
                .Select(x => new {S = x.Sum(rp => rp.D)})
                .ToList()
                .Select(x=>x.S)
                .Max(),2);
        }

        private double GetMinMoneySpentInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(_dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => new { rp.ReceiptID, D = rp.Price * rp.Amount })
                .GroupBy(rp => rp.ReceiptID)
                .Select(x => new { S = x.Sum(rp => rp.D) })
                .ToList()
                .Select(x => x.S)
                .Min(), 2);
        }

        private double GetMaxProductsAmountBoughtInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(_dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => new {R= rp.ReceiptID,P= rp.Amount})
                .GroupBy(rp => rp.R)
                .Select(x => new { S = x.Max(rp => rp.P) })
                .ToList()
                .Select(x => x.S)
                .Max(), 2);
        }

        private double GetMinProductsAmountBoughtInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(_dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => new { R = rp.ReceiptID, P = rp.Amount })
                .GroupBy(rp => rp.R)
                .Select(x => new { S = x.Min(rp => rp.P) })
                .ToList()
                .Select(x => x.S)
                .Min(), 2);
        }

        private async Task<double> GetTotalProductsAmountBoughtInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .SumAsync(rp => rp.Amount), 2);
        }

        private int GetMaxProductsBoughtInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => new {R = rp.ReceiptID, P = rp.ProductID})
                .GroupBy(rp => rp.R)
                .Select(x=>new {S=x.Count()})
                .ToList()
                .Select(x=>x.S)
                .Max();
        }

        private int GetMinProductsBoughtInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => new { R = rp.ReceiptID, P = rp.ProductID })
                .GroupBy(rp => rp.R)
                .Select(x => new { S = x.Count() })
                .ToList()
                .Select(x => x.S)
                .Min();
        }

        private async Task<double> GetAverageProductsBoughtInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round((double)await GetTotalProductsBoughtInShop(userId, shopId, startDate, endDate) /
                   await GetVisitAmount(userId, shopId, startDate, endDate),2);
        }

        private async Task<double> GetAverageProductsAmountBoughtInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await GetTotalProductsAmountBoughtInShop(userId, shopId, startDate, endDate) /
                   await GetVisitAmount(userId, shopId, startDate, endDate),2);
        }

        private async Task<int> GetTotalProductsBoughtInShop(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp=>rp.ProductID)
                .Distinct()
                .CountAsync();
        }

        private async Task<double> GetAvarageMoneySpentInShop(int userId, int shopId, DateTime startDate,
            DateTime endDate)
        {
            return Math.Round(
                await GetTotalSpentAmount(userId, shopId, startDate, endDate) /
                await GetVisitAmount(userId, shopId, startDate, endDate), 2);
        }

        private async Task<int> GetAmountOfAllCategories(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ProductCategories
                .Include(pc => pc.Product)
                .ThenInclude(p => p.ReceiptProducts)
                .ThenInclude(rp => rp.Receipt)
                .Where(pc => pc.Product.ReceiptProducts
                    .Any(rp => rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId
                               && rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate))
                .Select(pc => pc.CategoryID)
                .Distinct()
                .CountAsync();
        }

        private int GetMinAmountOfAllCategories(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => new { R = rp.ReceiptID, P = rp.Product.ProductCategories.Count })
                .GroupBy(rp => rp.R)
                .Select(x => new { S = x.Distinct().Sum(rp=>rp.P) })
                .ToList()
                .Select(x => x.S)
                .Min();
        }

        private int GetMaxAmountOfAllCategories(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => new { R = rp.ReceiptID, P = rp.Product.ProductCategories.Count })
                .GroupBy(rp => rp.R)
                .Select(x => new { S = x.Distinct().Sum(rp => rp.P) })
                .ToList()
                .Select(x => x.S)
                .Max();
        }

        private async Task<List<int>> GetAllCategories(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ProductCategories
                .Include(pc => pc.Product)
                .ThenInclude(p=>p.ReceiptProducts)
                .ThenInclude(rp=>rp.Receipt)
                .Where(pc => pc.Product.ReceiptProducts.Any(rp=>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate))
                .Select(rp => rp.CategoryID)
                .Distinct()
                .ToListAsync();
        }

        private async Task<List<ChartData>> GetAverageMoneySpentOnCategories(int userId, int shopId, 
            DateTime startDate, DateTime endDate)
        {
            var result = new List<ChartData>();
            var allCategories = await GetAllCategories(userId, shopId, startDate, endDate);

            foreach (var category in allCategories)
            {
                result.Add(new ChartData()
                {
                    Name = await _dbHelper.GetCategoryName(category),
                    Value = await GetMoneySpentOnCategoryInShop(userId, shopId, category, startDate, endDate) /
                            await GetHowManyTimesCategoryWasBoughtInShop(userId, shopId, category, startDate, endDate)
                });
            }
            return result;
        }

        private async Task<double> GetMoneySpentOnCategoryInShop(int userId, int shopId, int categoryId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Include(rp => rp.Product)
                .ThenInclude(p => p.ProductCategories)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate && rp.Product.ProductCategories.Any(pc=>pc.CategoryID == categoryId))
                .Select(rp => rp.Amount * rp.Price)
                .SumAsync(),2);
        }

        private async Task<double> GetHowManyTimesCategoryWasBoughtInShop(int userId, int shopId, int categoryId, DateTime startDate, DateTime endDate)
        {
            return Math.Round((double)await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Include(rp => rp.Product)
                .ThenInclude(p => p.ProductCategories)
                .Where(rp =>
                    rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId &&
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate && rp.Product.ProductCategories.Any(pc => pc.CategoryID == categoryId))
                .CountAsync(),2);
        }

        private async Task<int> GetMostPopularCategory(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            var allCategories = await GetAllCategories(userId, shopId, startDate, endDate);
            int result=-1;
            double lastValue = 0;
            foreach (var category in allCategories)
            {
                var times = await GetHowManyTimesCategoryWasBoughtInShop(userId, shopId, category, startDate, endDate);
                if (times >= lastValue)
                {
                    lastValue = times;
                    result = category;
                }
            }
            return result;
        }

        private async Task<int> GetLeastPopularCategory(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            var allCategories = await GetAllCategories(userId, shopId, startDate, endDate);

            double lastValue = await GetHowManyTimesCategoryWasBoughtInShop(userId, shopId, allCategories[0], startDate, endDate);
            int result = allCategories[0];

            for (var i = 1; i < allCategories.Count; i++)
            {
                var category = allCategories[i];
                var times = await GetHowManyTimesCategoryWasBoughtInShop(userId, shopId, category, startDate, endDate);
                if (times <= lastValue)
                {
                    lastValue = times;
                    result = category;
                }
            }
            return result;
        }

        private async Task<double> GetAverageMoneySpentInShopPerDay(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await GetTotalSpentAmount(userId, shopId, startDate, endDate) /
                   _dbHelper.GetDaysBetweenDates(startDate, endDate),2);
        }

        private async Task<double> GetAverageAmountOfCategories(int userId, int shopId, DateTime startDate, DateTime endDate)
        {
            return Math.Round((double)await GetAmountOfAllCategories(userId, shopId, startDate, endDate) /
                   await GetVisitAmount(userId, shopId, startDate, endDate),2);
        }
    }

    public interface IShopSummaryHelper
    {
        Task<ShopSummary> GetShopSummary(int userId, int shopId, string startDate, string endDate);
    }
}
