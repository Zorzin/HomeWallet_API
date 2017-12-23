using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using HomeWallet_API.Models.GET;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class PlanHelper : IPlanHelper
    {
        private readonly DBContext _context;
        public PlanHelper(DBContext context)
        {
            _context = context;
        }

        public async Task<Plan> GetPlanForDate(int userId, string date)
        {

            var searchingDate = DateTime.Parse(date);
            return await _context.Plans.FirstOrDefaultAsync(m => m.UserID == userId && m.StartDate <= searchingDate && m.EndDate >= searchingDate);
        }

        public async Task<Plan> GetPlan(int userId, int id)
        {
            return await _context.Plans.FirstOrDefaultAsync(m => m.UserID == userId && m.ID == id);
        }

        public PlanDetails GetPlanDetails(Plan plan)
        {
            var planDetails = new PlanDetails()
            {
                AlreadySpent = GetAlreadySpent(plan.UserID,plan.StartDate,plan.EndDate),
                AlreadySpentPercent = GetPercentageSpent(plan.UserID,plan.StartDate,plan.EndDate,plan.Amount),
                Amount = plan.Amount,
                AverageLeft = GetAveragePerDay(plan.UserID,plan.StartDate,plan.EndDate,plan.Amount),
                DaysLeft = GetDaysLeft(plan.EndDate),
                EndDate = plan.EndDate,
                ID = plan.ID,
                MoneyLeft = GetMoneyLeft(plan.UserID,plan.StartDate,plan.EndDate,plan.Amount),
                StartDate = plan.StartDate,
                DaysBehind = GetDaysBehind(plan.StartDate,plan.EndDate)
            };


            return planDetails;
        }

        private double GetAlreadySpent(int userid, DateTime startDate, DateTime endDate)
        {
            var receipts = _context.Receipts.Where(r => r.UserID == userid).Include(r => r.ReceiptProducts).ToList();
            receipts = receipts.Where(r => r.PurchaseDate >= startDate && r.PurchaseDate < endDate).ToList();
            return receipts.SelectMany(receipt => receipt.ReceiptProducts).Sum(product => product.Amount * product.Price);
        }

        private double GetPercentageSpent(int userid, DateTime startDate, DateTime endDate, double plan)
        {
            var already = GetAlreadySpent(userid, startDate, endDate);
            var result = 100 * already / plan;
            return Math.Round(result, 1);
        }

        private double GetMoneyLeft(int userid, DateTime startDate, DateTime endDate, double plan)
        {
            var already = GetAlreadySpent(userid, startDate, endDate);
            return plan - already;
        }

        private int GetDaysLeft(DateTime end)
        {
            return (end - DateTime.Today).Days;
        }

        private int GetDaysBehind(DateTime start, DateTime end)
        {
            var left = GetDaysLeft(end);
            var alldays = (end - start).Days;
            return alldays - left;
        }

        private double GetAveragePerDay(int userid, DateTime startDate, DateTime endDate,  double plan)
        {
            var already = GetMoneyLeft(userid, startDate, endDate, plan);
            var days = GetDaysLeft(endDate);
            return Math.Round(already / days, 2);
        }

    }

    public interface IPlanHelper
    {
        PlanDetails GetPlanDetails(Plan plan);
        Task<Plan> GetPlanForDate(int userId, string date);
        Task<Plan> GetPlan(int userId, int id);
    }
}

