using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;

namespace HomeWallet_API.Logic
{
    public class PlanSummaryHelper : IPlanSummaryHelper
    {
        private readonly DBContext _dbContext;

        public PlanSummaryHelper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<PlanSummary> GetPlanSummary(int userId, int planId, string startDate, string endDate)
        {
            return null;
        }
    }

    public interface IPlanSummaryHelper
    {
        Task<PlanSummary> GetPlanSummary(int userId, int planId, string startDate, string endDate);
    }
}
