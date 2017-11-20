using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;

namespace HomeWallet_API.Logic
{
    public class CreateProduct
    {
        public static void CreateProductCategories(ICollection<int> categories, int productID, DBContext context)
        {
            foreach (var category in categories)
            {
                var productcategory = new ProductCategory()
                {
                    ProductID = productID,
                    CategoryID = category
                };
                context.ProductCategories.Add(productcategory);
                context.SaveChanges();
            }
        }

        public static void CreateReceiptProducts(ICollection<ReceiptProductPost> products, int receiptId, DBContext context)
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
                context.ReceiptProducts.Add(receiptproduct);
                context.SaveChanges();
            }
        }

        public static Receipt CreateReceipt(int shopID, int userID, DateTime date, DBContext context)
        {
            var receipt = new Receipt()
            {
                ShopID = shopID,
                UserID = userID,
                PurchaseDate = date
            };
            context.Receipts.Add(receipt);
            context.SaveChanges();
            return receipt;
        }

        public static Product Create(string name, int userid, DBContext context)
        {
            var product = new Product()
            {
                Name = name,
                UserID = userid
            };
            context.Products.Add(product);
            context.SaveChanges();
            return product;
        }
    }
}
