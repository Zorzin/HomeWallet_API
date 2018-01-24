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
        private readonly DBContext _dbContext;
        private readonly IDbHelper _dbHelper;

        public SummaryHelper(DBContext dbContext, IDbHelper dbHelper)
        {
            _dbContext = dbContext;
            _dbHelper = dbHelper;
        }

        public async Task<Summary> GetSummary(int userId, string startDateString, string endDateString)
        {
            var startDate = DateTime.Parse(startDateString);
            var endDate = DateTime.Parse(startDateString);
            var totalMoney = await GetAllMoney(userId, startDate, endDate);
            var percentPlan = await GetPercent(userId, startDate, endDate, totalMoney);
            var productsCost = await _dbHelper.GetProductsCost(userId, startDate, endDate);
            var minimumCost = Math.Round(productsCost.Min(),2);
            var maxCost = Math.Round(productsCost.Max(), 2);
            var averageCost = Math.Round(productsCost.Average(), 2);
            var summary = new Summary()
            {
                StartDate = startDateString,
                EndDate = endDateString,
                PercentPlan = percentPlan,
                TotalCost = totalMoney,
                ShopsCount = await GetAllShopsCount(userId, startDate, endDate),
                ProductsCount = await GetAllProductsCount(userId, startDate, endDate),
                CategoriesCount = GetAllCategoriesCount(userId, startDate, endDate),
                EachCategoryCost = await GetMoneySpentOnEachCategory(userId, startDate, endDate),
                EachShopMoney = await GetMoneySpentInEachShop(userId, startDate, endDate),
                EachShopProducts = await GetProductsAmountForEachShop(userId, startDate, endDate),
                MaxProductCost = maxCost,
                MinProductCost = minimumCost,
                AverageProductCost = averageCost,
                AverageReceiptCost = await GetAverageReceiptCost(userId, startDate, endDate)
            };
            return summary;
        }

        private async Task<List<ChartData>> GetProductsAmountForEachShop(int userId, DateTime startDate, DateTime endDate)
        {
            var productsInShops = new List<ChartData>();
            var shops = await GetAllShops(userId, startDate, endDate);
            foreach (var shop in shops)
            {
                productsInShops.Add(new ChartData()
                {
                    Name = _dbHelper.GetShopName(shop),
                    Value = await GetProductAmountBoughtInShop(userId, startDate, endDate, shop)
                });
            }
            return productsInShops;
        }

        private async Task<List<ChartData>> GetMoneySpentInEachShop(int userId, DateTime startDate, DateTime endDate)
        {
            var shopMoney = new List<ChartData>();
            var shops = await GetAllShops(userId, startDate, endDate);
            foreach (var shop in shops)
            {
                shopMoney.Add(new ChartData()
                {
                    Name = _dbHelper.GetShopName(shop),
                    Value = await GetMoneySpentInShop(userId, startDate, endDate, shop)
                });
            }
            return shopMoney;
        }

        private async Task<List<ChartData>> GetMoneySpentOnEachCategory(int userId, DateTime startDate, DateTime endDate)
        {
            var categoriesMoney = new List<ChartData>();
            var categories = await GetAllCategories(userId, startDate, endDate);
            foreach (var category in categories)
            {
                categoriesMoney.Add(new ChartData()
                {
                    Name = _dbHelper.GetCategoryName(category),
                    Value = await GetMoneySpentOnCategory(userId, startDate, endDate, category)
                });
            }
            return categoriesMoney;
        }

        private async Task<double> GetAverageReceiptCost(int userId, DateTime startDate, DateTime endDate)
        {
            var total = await GetAllMoney(userId, startDate, endDate);
            var receipt = await GetReceiptAmount(userId, startDate, endDate);
            return Math.Round(total / receipt,2);
        }

        private async Task<int> GetReceiptAmount(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Receipts.CountAsync(r => r.UserID == userId && r.PurchaseDate >= startDate && r.PurchaseDate <= endDate);
        }

        private async Task<int> GetProductAmountBoughtInShop(int userId, DateTime startDate, DateTime endDate, int shopId)
        {
            return await _dbContext.ReceiptProducts
                .Include(r => r.Receipt)
                .Where(r => r.Receipt.PurchaseDate >= startDate && r.Receipt.PurchaseDate <= endDate &&
                            r.Receipt.UserID == userId && r.Receipt.ShopID == shopId)
                .Select(r => r.ProductID)
                .CountAsync();
        }

        private async Task<double> GetMoneySpentOnCategory(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Include(p => p.Product)
                .ThenInclude(pc => pc.ProductCategories)
                .Where(rp =>
                    rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate && rp.Product.ProductCategories.Any(pc=>pc.CategoryID==categoryId))
                .Select(rp => rp.Amount * rp.Price)
                .SumAsync(),2);
        }

        private async Task<double> GetMoneySpentOnProduct(int userId, DateTime startDate, DateTime endDate, int productId)
        {
            return Math.Round(await _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Include(r => r.ReceiptProducts)
                .Select(r => r.ReceiptProducts
                    .Where(rp => rp.ProductID==productId)
                    .Select(rp => rp.Amount * rp.Price)
                    .Sum())
                .SumAsync(),2);
        }

        private async Task<double> GetMoneySpentInShop(int userId, DateTime startDate, DateTime endDate, int shopId)
        {
            return Math.Round(await _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId && r.ShopID == shopId)
                .Include(r=>r.ReceiptProducts)
                .Select(r => r.ReceiptProducts
                    .Select(rp=>rp.Amount * rp.Price)
                    .Sum())
                .SumAsync(),2);
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

        private async Task<double> GetAllMoney(int userId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Include(r => r.ReceiptProducts)
                .Select(r=>r.ReceiptProducts
                    .Select(rp=>rp.Amount*rp.Price)
                    .Sum())
                .SumAsync(),2);
        }

        private async Task<int> GetAllShopsCount(int userId, DateTime startDate, DateTime endDate)
        {
            return (await GetAllShops(userId,startDate,endDate)).Count;
        }

        private async Task<List<int>> GetAllShops(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Select(r => r.ShopID)
                .Distinct()
                .ToListAsync();
        }

        private async Task<int> GetAllProductsCount(int userId, DateTime startDate, DateTime endDate)
        {
            return (await GetAllProducts(userId, startDate, endDate)).Count;
        }

        private async Task<List<int>> GetAllProducts(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Receipts
                .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                .Include(r => r.ReceiptProducts)
                .Select(r => r.ReceiptProducts
                    .Select(rp => rp.ProductID)
                    .Distinct()
                    .ToList())
                .Distinct()
                .SelectMany(x => x)
                .ToListAsync();
        }

        private int GetAllCategoriesCount(int userId, DateTime startDate, DateTime endDate)
        {
            return GetAllCategories(userId, startDate, endDate).Result.Count;
        }

        private async Task<List<int>> GetAllCategories(int userId, DateTime startDate, DateTime endDate)
        {
            var receiptList = await _dbContext.Receipts
                            .Where(r => r.PurchaseDate >= startDate && r.PurchaseDate <= endDate && r.UserID == userId)
                            .Include(r => r.ReceiptProducts)
                            .ThenInclude(rp => rp.Product)
                            .ThenInclude(p => p.ProductCategories)
                            .Select(x => x.ReceiptProducts
                                .SelectMany(rp => rp.Product.ProductCategories
                                    .Select(pc => pc.CategoryID))
                                .Distinct()
                                .ToList())
                            .ToListAsync();

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
        Task<Summary> GetSummary(int userId, string startDate, string endDate);
    }
}
