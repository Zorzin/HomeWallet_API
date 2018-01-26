using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class ProductSummaryHelper : IProductSummaryHelper
    {
        private readonly DBContext _dbContext;
        private readonly IDbHelper _dbHelper;

        public ProductSummaryHelper(DBContext dbContext, IDbHelper dbHelper)
        {
            _dbContext = dbContext;
            _dbHelper = dbHelper;
        }

        public async Task<ProductSummary> GetProductSummary(int userId, int productId, string startDate, string endDate)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            var productSummary = new ProductSummary()
            {
                AverageAmountOnReceipt = await GetAverageBoughtAmountOfProduct(userId,productId,start,end),
                AverageCostPerDay = await GetAverageCostPerDay(userId,productId,start,end),
                AveragePrice = await GetAverageProductPrice(userId,productId,start,end),
                AveragePriceInShops = await GetAveragePriceInShopsAsChartData(userId,productId,start,end),
                MaxAmountOnReceipt = await GetMaxBoughtAmountOfProduct(userId,productId,start,end),
                MaxPrice = (await GetProductPrices(userId,productId,start,end)).Max(),
                MinAmountOnReceipt = await GetMinBoughtAmountOfProduct(userId,productId,start,end),
                MinPrice = (await GetProductPrices(userId, productId, start, end)).Min(),
                PercentageAmountOfTotalMoneySpent = await GetPercentageMoneySpentOnThisProductComparedToEverything(userId,productId,start,end),
                ShopsAmount = await GetShopsCount(userId,productId,start,end),
                ShopWithCheapestPrice = (await GetShopWithLowestPrice(userId,productId,start,end)).ShopId,
                ShopWithMostExpensivePrice = (await GetShopWithHighestPrice(userId, productId, start, end)).ShopId,
                TotalMoneySpent = await GetMoneySpentOnProduct(userId,productId,start,end),
                TotalTimesBoughtAmount = await GetTimesBought(userId,productId,start,end)
            };
            return productSummary;
        }

        private async Task<double> GetAverageCostPerDay(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            var totalSpentOnProduct = await GetMoneySpentOnProduct(userId, productId, startDate, endDate);
            var daysBetweenDates = _dbHelper.GetDaysBetweenDates(startDate, endDate);
            return Math.Round(totalSpentOnProduct/daysBetweenDates, 2);

        }

        private async Task<double> GetPercentageMoneySpentOnThisProductComparedToEverything(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            var totalSpentAtEverything = await _dbHelper.GetTotalSpentBetweenDates(userId, startDate, endDate);
            var totalSpentOnProduct = await GetMoneySpentOnProduct(userId, productId, startDate, endDate);
            return Math.Round((totalSpentOnProduct * 100) / totalSpentAtEverything,2);

        }

        private async Task<int> GetTimesBought(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.ProductID == productId && rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.ProductID)
                .CountAsync();
        }

        private async Task<double> GetTotalBoughtAmountOfProduct(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
           return Math.Round( await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp => rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                             rp.Receipt.PurchaseDate <= endDate && rp.ProductID == productId)
                .Select(r => r.Amount)
                .SumAsync(),2);
        }

        private async Task<double> GetAverageBoughtAmountOfProduct(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return Math.Round(
                await GetTotalBoughtAmountOfProduct(userId, productId, startDate, endDate) /
                await GetTimesBought(userId, productId, startDate, endDate), 2);
        }

        private async Task<double> GetMaxBoughtAmountOfProduct(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp => rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                             rp.Receipt.PurchaseDate <= endDate && rp.ProductID == productId)
                .Select(r => r.Amount)
                .MaxAsync(), 2);
        }

        private async Task<double> GetMinBoughtAmountOfProduct(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp => rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                             rp.Receipt.PurchaseDate <= endDate && rp.ProductID == productId)
                .Select(r => r.Amount)
                .MinAsync(), 2);
        }

        private async Task<ShopPrice> GetShopWithHighestPrice(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            var shopsPrice = await GetShopsPrice(userId, productId, startDate, endDate);

            var maxPrice = shopsPrice.Select(s => s.Price).Max();
            return new ShopPrice()
            {
                Price = maxPrice,
                ShopId = shopsPrice.FirstOrDefault(s=>s.Price==maxPrice).ShopId
            };
        }
        private async Task<ShopPrice> GetShopWithLowestPrice(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            var shopsPrice = await GetShopsPrice(userId, productId, startDate, endDate);

            var maxPrice = shopsPrice.Select(s => s.Price).Min();
            return new ShopPrice()
            {
                Price = maxPrice,
                ShopId = shopsPrice.FirstOrDefault(s => s.Price == maxPrice).ShopId
            };
        }

        private async Task<List<ChartData>> GetAveragePriceInShopsAsChartData(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            var averagePrices = await GetAveragePriceInShops(userId, productId, startDate, endDate);
            var result = new List<ChartData>();
            foreach (var averagePrice in averagePrices)
            {
                result.Add(new ChartData()
                {
                    Name = await _dbHelper.GetShopName(averagePrice.ShopId),
                    Value = Math.Round(averagePrice.Price,2)
                });
            }
            return result;
        }

        private async Task<List<ShopPrice>> GetAveragePriceInShops(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            var result = new List<ShopPrice>();
            var shopsPrice = await GetShopsPrice(userId, productId, startDate, endDate);

            foreach (var shopPrice in shopsPrice)
            {
                var shop = result.FirstOrDefault(r => r.ShopId == shopPrice.ShopId);
                if (shop!=null)
                {
                    shop.Price += shopPrice.Price;
                }
                else
                {
                    result.Add(new ShopPrice()
                    {
                        Price = shopPrice.Price,
                        ShopId = shopPrice.ShopId
                    });
                }
            }

            foreach (var shop in result)
            {
                var shopCount = shopsPrice.Count(x => x.ShopId == shop.ShopId);
                shop.Price = shop.Price / shopCount;
            }

            return result;
        }

        private async Task<List<ShopPrice>> GetLowestPriceInShops(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            var result = new List<ShopPrice>();
            var shopsPrice = await GetShopsPrice(userId, productId, startDate, endDate);

            foreach (var shopPrice in shopsPrice)
            {
                var shop = result.FirstOrDefault(r => r.ShopId == shopPrice.ShopId);
                if (shop != null)
                {
                    if (shop.Price > shopPrice.Price)
                    {
                        shop.Price = shopPrice.Price;
                    }
                }
                else
                {
                    result.Add(new ShopPrice()
                    {
                        Price = shopPrice.Price,
                        ShopId = shopPrice.ShopId
                    });
                }
            }
            return result;
        }

        private async Task<List<ShopPrice>> GetHighestPriceInShops(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            var result = new List<ShopPrice>();
            var shopsPrice = await GetShopsPrice(userId, productId, startDate, endDate);

            foreach (var shopPrice in shopsPrice)
            {
                var shop = result.FirstOrDefault(r => r.ShopId == shopPrice.ShopId);
                if (shop != null)
                {
                    if (shop.Price < shopPrice.Price)
                    {
                        shop.Price = shopPrice.Price;
                    }
                }
                else
                {
                    result.Add(new ShopPrice()
                    {
                        Price = shopPrice.Price,
                        ShopId = shopPrice.ShopId
                    });
                }
            }
            return result;
        }

        private async Task<List<ShopPrice>> GetShopsPrice(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp => rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                             rp.Receipt.PurchaseDate <= endDate && rp.ProductID == productId)
                .Select(r => new ShopPrice()
                {
                    ShopId = r.Receipt.ShopID,
                    Price  = r.Price
                })
                .ToListAsync();
        }

        private async Task<List<int>> GetShops(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Receipts
                .Include(r => r.ReceiptProducts)
                .Where(r => r.UserID == userId && r.PurchaseDate >= startDate &&
                            r.PurchaseDate <= endDate && r.ReceiptProducts
                                .Any(rp => rp.ProductID == productId))
                .Select(r => r.ShopID)
                .Distinct()
                .ToListAsync();
        }

        private async Task<int> GetShopsCount(int userId, int productId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Receipts
                .Include(r => r.ReceiptProducts)
                .Where(r => r.UserID == userId && r.PurchaseDate >= startDate &&
                            r.PurchaseDate <= endDate && r.ReceiptProducts
                                .Any(rp => rp.ProductID == productId))
                .Select(r => r.ShopID)
                .Distinct()
                .CountAsync();
        }

        private async Task<double> GetMoneySpentOnProduct(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.ProductID == productId && rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.Amount * rp.Price)
                .SumAsync(),2);
        }

        private async Task<double> GetAverageProductPrice(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return Math.Round(await GetProductPricesSum(userId, productId, startDate, endDate)/
                   await GetTimesBought(userId, productId, startDate, endDate),2);
        }

        private async Task<List<double>> GetProductPrices(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.ProductID == productId && rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.Price)
                .ToListAsync();
        }

        private async Task<double> GetProductPricesSum(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.ProductID == productId && rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.Price)
                .SumAsync();
        }

        private async Task<double> GetHighestPriceForProduct(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return Math.Round( await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.ProductID == productId && rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.Price)
                .OrderByDescending(x=>x)
                .FirstOrDefaultAsync(),2);
        }

        private async Task<double> GetLowerPriceForProduct(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return Math.Round( await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.ProductID == productId && rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.Price)
                .OrderBy(x => x)
                .FirstOrDefaultAsync(),2);
        }
    }

    public interface IProductSummaryHelper
    {
        Task<ProductSummary> GetProductSummary(int userId, int productId, string startDate, string endDate);
    }

    public class ShopPrice
    {
        public int ShopId { get; set; }
        public double Price { get; set; }
    }
}
