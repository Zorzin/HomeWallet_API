using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class ProductHelper : IProductHelper
    {
        private readonly DBContext _context;

        public ProductHelper(DBContext context)
        {
            _context = context;
        }

        public void CreateProductCategories(ICollection<int> categories, int productID)
        {
            foreach (var category in categories)
            {
                var productcategory = new ProductCategory()
                {
                    ProductID = productID,
                    CategoryID = category
                };
                _context.ProductCategories.Add(productcategory);
                _context.SaveChanges();
            }
        }

        public void CreateReceiptProducts(ICollection<ReceiptProductPost> products, int receiptId)
        {
            foreach (var product in products)
            {
                var receiptproduct = new ReceiptProduct()
                {
                    ReceiptID = receiptId,
                    ProductID = product.ProductId,
                    Amount = product.Amount,
                    Price = product.Price
                };
                _context.ReceiptProducts.Add(receiptproduct);
                _context.SaveChanges();
            }
        }

        public Receipt CreateReceipt(int shopID, int userID, DateTime date)
        {
            var receipt = new Receipt()
            {
                ShopID = shopID,
                UserID = userID,
                PurchaseDate = date
            };
            _context.Receipts.Add(receipt);
            _context.SaveChanges();
            return receipt;
        }

        public Product Create(string name, int userid)
        {
            var product = new Product()
            {
                Name = name,
                UserID = userid
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public List<Category> GetCategories(int userId, int id)
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

        public void UpdateProduct(int userId, int id, ProductPost product)
        {
            var productObj = _context.Products.FirstOrDefault(p => p.ID == id && p.UserID == userId);

            productObj.Name = product.name;
            _context.SaveChanges();
            DeleteOldCategories(productObj, product.categories);
            AddNewCategories(productObj, product.categories);
        }

        private void AddNewCategories(Product productObj, int[] productCategories)
        {
            var dbCategories = _context.ProductCategories.Where(p => p.ProductID == productObj.ID).Select(p=>p.CategoryID).ToList();
            foreach (var category in productCategories)
            {
                if (!dbCategories.Contains(category))
                {
                    _context.ProductCategories.Add(new ProductCategory()
                    {
                        CategoryID = category,
                        ProductID = productObj.ID
                    });
                }
            }
            _context.SaveChanges();
        }

        private void DeleteOldCategories(Product productObj, int[] productCategories)
        {
            var dbCategories = _context.ProductCategories.Where(p => p.ProductID == productObj.ID).ToList();

            for (var i = dbCategories.Count()-1; i >= 0; i--)
            {
                var pc = dbCategories.ElementAt(i);
                if (!productCategories.Contains(pc.CategoryID))
                {
                    _context.ProductCategories.Remove(pc);
                }
            }
            _context.SaveChanges();
        }
    }

    public interface IProductHelper
    {
        void CreateProductCategories(ICollection<int> categories, int productID);
        void CreateReceiptProducts(ICollection<ReceiptProductPost> products, int receiptId);
        Receipt CreateReceipt(int shopID, int userID, DateTime date);
        Product Create(string name, int userid);
        List<Category> GetCategories(int userId, int id);
        void UpdateProduct(int userId, int id, ProductPost product);

    }
}
