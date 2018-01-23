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

        public Task<ProductSummary> GetProductSummary(int userId, int productId, string startDate, string endDate)
        {
            return null;
        }

        private async Task<double> GetAvarageCostPerDay(int userId, int productId, DateTime startDate, DateTime endDate)
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

        private async Task<double> GetAvarageBoughtAmountOfProduct(int userId, int productId, DateTime startDate,
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

        private async Task<List<ShopPrice>> GetAvaragePriceInShops(int userId, int productId, DateTime startDate, DateTime endDate)
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

        private async Task<double> GetAverageProductPride(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return await GetProductPricesCount(userId, productId, startDate, endDate)/
                   await GetTimesBought(userId, productId, startDate, endDate);
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

        private async Task<double> GetProductPricesCount(int userId, int productId, DateTime startDate,
            DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.ProductID == productId && rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.Price)
                .CountAsync();
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
