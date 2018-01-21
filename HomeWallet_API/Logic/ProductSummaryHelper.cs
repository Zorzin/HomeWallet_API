using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;

namespace HomeWallet_API.Logic
{
    public class ProductSummaryHelper : IProductSummaryHelper
    {
        private readonly DBContext _dbContext;

        public ProductSummaryHelper(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ProductSummary> GetProductSummary(int userId, int productId, string startDate, string endDate)
        {
            return null;
        }
    }

    public interface IProductSummaryHelper
    {
        Task<ProductSummary> GetProductSummary(int userId, int productId, string startDate, string endDate);
    }
}
