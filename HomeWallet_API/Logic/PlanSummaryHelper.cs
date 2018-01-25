using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class PlanSummaryHelper : IPlanSummaryHelper
    {
        private readonly DBContext _dbContext;
        private readonly IDbHelper _dbHelper;

        public PlanSummaryHelper(DBContext dbContext, IDbHelper dbHelper)
        {
            _dbContext = dbContext;
            _dbHelper = dbHelper;
        }

        public async Task<PlanSummary> GetPlanSummary(int userId, int planId)
        {

            var dates = await GetPlanDates(userId, planId);
            var startDate = dates.StartDate;
            var endDate = dates.EndDate;

            var planSummary = new PlanSummary()
            {
                Amount = await GetPlanAmount(userId, planId),
                AverageMoneyLeftPerDay = await GetAverageMoneyLeftPerDayForPlan(userId, planId, startDate, endDate),
                AverageMoneySpentPerDay = await GetAverageMoneySpentPerDayForPlan(userId, startDate, endDate),
                MoneyLeft = await GetMoneyLeftForPlan(userId, planId, startDate, endDate),
                MoneyLeftPercentage = await GetMoneyLeftForPlanPercentage(userId, planId, startDate, endDate),
                MoneySpentOnCategories = await GetMoneySpentOnCategories(userId, startDate, endDate),
                MoneySpentPercentage = await GetMoneySpentForPlanPercentage(userId, planId, startDate, endDate),
                MostExpensiveProduct = await GetMostExpensiveProduct(userId, startDate, endDate),
                MostPopularCategory = await GetMostPopularCategoryForPlan(userId, startDate, endDate),
                MostPopularProduct = await GetMostPopularProductForPlan(userId, startDate, endDate),
                ProductsBoughtAmount = await GetProductsAmountForPlan(userId, startDate, endDate)
            };
            return planSummary;
        }

        private async Task<int> GetMostExpensiveProduct(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate &&
                    rp.Receipt.UserID == userId)
                .Select(rp => new {P = rp.Price,I=rp.ProductID})
                .OrderByDescending(x=>x.P)
                .Select(x=>x.I)
                .FirstOrDefaultAsync();
        }

        private async Task<List<ChartData>> GetMoneySpentOnCategories(int userId, DateTime startDate, DateTime endDate)
        {
            var categories = await GetCategoriesForPlan(userId, startDate, endDate);
            var result = new List<ChartData>();

            foreach (var category in categories)
            {
                result.Add(new ChartData()
                {
                    Name = await _dbHelper.GetCategoryName(category),
                    Value = await GetMoneySpentOnCategoryForPlan(userId,startDate,endDate,category)
                });
            }
            return result;
        }

        private async Task<double> GetPlanAmount(int userId, int planId)
        {
            return Math.Round(await _dbContext.Plans
                .Where(p => p.UserID == userId && p.ID == planId)
                .Select(p => p.Amount)
                .FirstOrDefaultAsync(),2);
        }

        private async Task<double> GetMoneyLeftForPlan(int userId, int planId, DateTime startDate, DateTime endDate)
        {
            return Math.Round(await GetPlanAmount(userId, planId) - await GetMoneyAlreadySpentForPlan(userId, startDate, endDate), 2);
        }

        private async Task<PlanDates> GetPlanDates(int userId, int planId)
        {
            return await _dbContext.Plans.Where(p => p.UserID == userId && p.ID == planId)
                .Select(x => new PlanDates(){StartDate=x.StartDate, EndDate = x.EndDate}).FirstOrDefaultAsync();
        }

        private async Task<double> GetMoneyAlreadySpentForPlan(int userId, DateTime startDate, DateTime endDate)
        {

            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate &&
                    rp.Receipt.UserID == userId)
                .Select(rp => rp.Amount * rp.Price)
                .SumAsync(),2);
        }

        private async Task<double> GetMoneyLeftForPlanPercentage(int userId, int planId, DateTime startDate, DateTime endDate)
        {
            var left = await GetMoneyLeftForPlan(userId, planId, startDate, endDate);
            var all = await GetPlanAmount(userId, planId);
            return Math.Round((left * 100) / all,2);
        }

        private async Task<double> GetMoneySpentForPlanPercentage(int userId, int planId, DateTime startDate, DateTime endDate)
        {
            var spent = await GetMoneyAlreadySpentForPlan(userId, startDate, endDate);
            var all = await GetPlanAmount(userId, planId);
            return Math.Round((spent * 100) / all,2);
        }

        private async Task<int> GetMostPopularProductForPlan(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate)
                .Select(rp => rp.ProductID)
                .GroupBy(s => s)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key)
                .FirstOrDefaultAsync();
        }

        private async Task<int> GetMostPopularCategoryForPlan(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ProductCategories
                .Include(pc => pc.Product)
                .ThenInclude(p=>p.ReceiptProducts)
                .ThenInclude(rp=>rp.Receipt)
                .Where(pc =>
                    pc.Product.ReceiptProducts.Any(rp=>rp.Receipt.PurchaseDate>=startDate && rp.Receipt.PurchaseDate <= endDate && rp.Receipt.UserID == userId))
                .Select(pc => pc.CategoryID)
                .GroupBy(s => s)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key)
                .FirstOrDefaultAsync();
        }

        private async Task<List<int>> GetCategoriesForPlan(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ProductCategories
                .Include(pc => pc.Product)
                .ThenInclude(p => p.ReceiptProducts)
                .ThenInclude(rp => rp.Receipt)
                .Where(pc =>
                    pc.Product.ReceiptProducts.Any(rp =>
                        rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate &&
                        rp.Receipt.UserID == userId))
                .Select(pc => pc.CategoryID)
                .Distinct()
                .ToListAsync();
        }

        private async Task<double> GetMoneySpentOnCategoryForPlan(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            return Math.Round(await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Include(p => p.Product)
                .ThenInclude(pc => pc.ProductCategories)
                .Where(rp =>
                    rp.Receipt.UserID == userId && rp.Receipt.PurchaseDate >= startDate &&
                    rp.Receipt.PurchaseDate <= endDate && rp.Product.ProductCategories.Any(pc => pc.CategoryID == categoryId))
                .Select(rp => rp.Amount * rp.Price)
                .SumAsync(), 2);
        }

        private async Task<double> GetAverageMoneyLeftPerDayForPlan(int userId, int planId, DateTime startDate, DateTime endDate)
        {
            var moneyLeft = await GetMoneyLeftForPlan(userId, planId, startDate, endDate);
            var daysLeft = _dbHelper.GetDaysBetweenDates(startDate, endDate);
            return Math.Round(moneyLeft / daysLeft, 2);
        }

        private async Task<double> GetProductsAmountForPlan(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Where(rp =>
                    rp.Receipt.PurchaseDate >= startDate && rp.Receipt.PurchaseDate <= endDate &&
                    rp.Receipt.UserID == userId)
                .Select(rp => rp.ProductID)
                .Distinct()
                .SumAsync();
        }

        private async Task<double> GetAverageMoneySpentPerDayForPlan(int userId, DateTime startDate, DateTime endDate)
        {
            var moneySpent = await GetMoneyAlreadySpentForPlan(userId, startDate, endDate);
            var daysLeft = _dbHelper.GetDaysBetweenDates(startDate, endDate);
            return Math.Round(moneySpent / daysLeft, 2);
        }
    }

    public interface IPlanSummaryHelper
    {
        Task<PlanSummary> GetPlanSummary(int userId, int planId);
    }

    public class PlanDates
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
