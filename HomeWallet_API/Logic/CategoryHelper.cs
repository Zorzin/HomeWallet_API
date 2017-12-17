using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class CategoryHelper : ICategoryHelper
    {
        private readonly DBContext _context;

        public CategoryHelper(DBContext context)
        {
            _context = context;
        }

        public List<Product> GetCategoryProducts(int categoryId, int userId)
        {
            var category = _context.Categories
                .Include(c=>c.ProductCategories)
                .ThenInclude(p => p.Product)
                .SingleOrDefault(m => m.ID == categoryId);
            if (category == null)
            {
                return null;
            }
            var products = new List<Product>();
            var productsId = new List<int>();
            foreach (var productCategory in category.ProductCategories)
            {
                if (!productsId.Contains(productCategory.ProductID))
                {
                    productsId.Add(productCategory.ProductID);
                    products.Add(productCategory.Product);
                    productCategory.Product.ProductCategories = null;
                }
            }
            return products;
        }

    }

    public interface ICategoryHelper
    {
        List<Product> GetCategoryProducts(int categoryId, int userId);
    }
}
