using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class ProductHelper
    {

        public static List<Category> GetCategories(DBContext _context, int userId, int id)
        {
            var product = _context.Products
                .Include(p=>p.ProductCategories)
                .ThenInclude(p=>p.Category)
                .FirstOrDefault(p => p.ID == id && p.UserID == userId);

            var categories = new List<Category>();

            foreach (var productCategory in product.ProductCategories)
            {
                productCategory.Category.ProductCategories = null;
                categories.Add(productCategory.Category);
            }

            return categories;
        }
    }
}
