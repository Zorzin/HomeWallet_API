using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class DbHelper : IDbHelper
    {
        private readonly DBContext _dbContext;

        public DbHelper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetShopName(int shop)
        {
            return await _dbContext.Shops.Where(s => s.ID == shop).Select(s => s.Name).FirstOrDefaultAsync();
        }

        public async Task<string> GetCategoryName(int category)
        {
            return await _dbContext.Categories.Where(c => c.ID == category).Select(c => c.Name).FirstOrDefaultAsync();
        }

        public int GetDaysBetweenDates(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).Days + 1;
        }


        public async Task<double> GetCheaperProductCost(int userId, DateTime startDate, DateTime endDate)
        {
            return Math.Round((await GetProductsCost(userId, startDate, endDate)).Min(), 2);
        }

        public async Task<double> GetMostExpensiveProductCost(int userId, DateTime startDate, DateTime endDate)
        {
            return Math.Round((await GetProductsCost(userId, startDate, endDate)).Max(), 2);
        }

        public async Task<double> GetAverageProductCost(int userId, DateTime startDate, DateTime endDate)
        {
            return Math.Round((await GetProductsCost(userId, startDate, endDate)).Average(), 2);
        }

        public async Task<List<double>> GetProductsCost(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate &&
                    rp.Receipt.UserID == userId)
                .Select(rp => rp.Price)
                .ToListAsync();
        }

        public async Task<double> GetTotalSpentBetweenDates(int userId, DateTime startDate, DateTime endDate)
        {
            return Math.Round( await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate &&
                    rp.Receipt.UserID == userId)
                .Select(rp => rp.Price * rp.Amount) 
                .SumAsync(),2);
        }

        public async Task<string> GetProductName(int id)
        {
            return await _dbContext.Products.Where(s => s.ID == id).Select(s => s.Name).FirstOrDefaultAsync();
        }
    }

    public interface IDbHelper
    {
        Task<string> GetShopName(int shop);
        Task<string> GetCategoryName(int category);
        int GetDaysBetweenDates(DateTime startDate, DateTime endDate);
        Task<List<double>> GetProductsCost(int userId, DateTime startDate, DateTime endDate);
        Task<double> GetAverageProductCost(int userId, DateTime startDate, DateTime endDate);
        Task<double> GetMostExpensiveProductCost(int userId, DateTime startDate, DateTime endDate);
        Task<double> GetCheaperProductCost(int userId, DateTime startDate, DateTime endDate);
        Task<double> GetTotalSpentBetweenDates(int userId, DateTime startDate, DateTime endDate);
        Task<string> GetProductName(int id);
    }
}
