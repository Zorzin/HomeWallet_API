using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class CategoryHelper
    {
        public static List<Product> GetCategoryProducts(int categoryId, int userId, DBContext context)
        {
            var category = context.Categories
                .Include(c=>c.ProductCategories)
                .ThenInclude(p => p.Product)
                .SingleOrDefault(m => m.ID == categoryId);
            if (category == null)
            {
                return null;
            }
            var products = new List<Product>();
            var productsID = new List<int>();
            foreach (var productCategory in category.ProductCategories)
            {
                if (!productsID.Contains(productCategory.ProductID))
                {
                    productsID.Add(productCategory.ProductID);
                    products.Add(productCategory.Product);
                    productCategory.Product.ProductCategories = null;
                }
            }

            return products;
        }

    }
}
