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

        public string GetShopName(int shop)
        {
            return _dbContext.Shops.Where(s => s.ID == shop).Select(s => s.Name).FirstOrDefault();
        }

        public string GetCategoryName(int category)
        {
            return _dbContext.Categories.Where(c => c.ID == category).Select(c => c.Name).FirstOrDefault();
        }

        public int GetDaysBetweenDates(DateTime startDate, DateTime endDate)
        {
            return (endDate - startDate).Days + 1;
        }


        public double GetCheaperProductCost(int userId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(GetProductsCost(userId, startDate, endDate).Result.Min(), 2);
        }

        public double GetMostExpensiveProductCost(int userId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(GetProductsCost(userId, startDate, endDate).Result
                .Max(), 2);
        }

        public double GetAverageProductCost(int userId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(GetProductsCost(userId, startDate, endDate).Result
                .Average(), 2);
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
    }

    public interface IDbHelper
    {
        string GetShopName(int shop);
        string GetCategoryName(int category);
        int GetDaysBetweenDates(DateTime startDate, DateTime endDate);
        Task<List<double>> GetProductsCost(int userId, DateTime startDate, DateTime endDate);
        double GetAverageProductCost(int userId, DateTime startDate, DateTime endDate);
        double GetMostExpensiveProductCost(int userId, DateTime startDate, DateTime endDate);
        double GetCheaperProductCost(int userId, DateTime startDate, DateTime endDate);
        Task<double> GetTotalSpentBetweenDates(int userId, DateTime startDate, DateTime endDate);
    }
}
