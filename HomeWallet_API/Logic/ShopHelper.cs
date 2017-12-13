using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class ShopHelper
    {

        public static List<Product> GetShopProducts(int shopId, int userId, DBContext context)
        {
            var shop = context.Shops
                .Include(s => s.Receipts)
                .ThenInclude(r => r.ReceiptProducts)
                .ThenInclude(rp => rp.Product)
                .SingleOrDefault(m => m.ID == shopId);
            if (shop == null)
            {
                return null;
            }
            var products = new List<Product>();
            var productsID = new List<int>();
            foreach (var shopReceipt in shop.Receipts)
            {
                foreach (var shopReceiptReceiptProduct in shopReceipt.ReceiptProducts)
                {
                    if (!productsID.Contains(shopReceiptReceiptProduct.ProductID))
                    {
                        productsID.Add(shopReceiptReceiptProduct.ProductID);
                        products.Add(shopReceiptReceiptProduct.Product);
                    }
                }
            }

            return products;
        }

    }
}
