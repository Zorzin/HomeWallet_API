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

        public static void UpdateProduct(DBContext _context, int userId, int id, ProductPost product)
        {
            var productObj = _context.Products.FirstOrDefault(p => p.ID == id && p.UserID == userId);

            productObj.Name = product.name;
            _context.SaveChanges();
            DeleteOldCategories(_context, productObj);
            AddNewCategories(_context, productObj, product.categories);
        }

        private static void AddNewCategories(DBContext _context, Product productObj, int[] productCategories)
        {
            foreach (var category in productCategories)
            {
                _context.ProductCategories.Add(new ProductCategory()
                {
                    CategoryID = category,
                    ProductID = productObj.ID
                });
            }
            _context.SaveChanges();
        }

        private static void DeleteOldCategories(DBContext _context, Product productObj)
        {
            var productCategories = _context.ProductCategories.Where(p => p.ProductID == productObj.ID).ToList();
            for (int i = productCategories.Count()-1; i >= 0; i--)
            {
                var pc = productCategories.ElementAt(i);
                _context.ProductCategories.Remove(pc);
            }
            _context.SaveChanges();
        }
    }
}
