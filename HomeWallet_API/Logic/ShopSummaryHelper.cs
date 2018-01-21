using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;

namespace HomeWallet_API.Logic
{
    public class ShopSummaryHelper : IShopSummaryHelper
    {
        private readonly DBContext _dbContext;

        public ShopSummaryHelper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ShopSummary> GetShopSummary(int userId, int shopId, string startDate, string endDate)
        {
            return null;
        }
    }

    public interface IShopSummaryHelper
    {
        Task<ShopSummary> GetShopSummary(int userId, int shopId, string startDate, string endDate);
    }
}
