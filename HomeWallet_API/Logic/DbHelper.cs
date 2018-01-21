using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;

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
    }

    public interface IDbHelper
    {
        string GetShopName(int shop);
        string GetCategoryName(int category);
    }
}
