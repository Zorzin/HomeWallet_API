using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeWallet_API.Logic
{
    public class ShopHelper : IShopHelper
    {
        private readonly DBContext _context;
        public ShopHelper(DBContext context)
        {
            _context = context;
        }

        public List<Product> GetShopProducts(int shopId, int userId)
        {
            var shop = _context.Shops
                .Include(s => s.Receipts)
                .ThenInclude(r => r.ReceiptProducts)
                .ThenInclude(rp => rp.Product)
                .SingleOrDefault(m => m.ID == shopId);
            if (shop == null)
            {
                return null;
            }
            var products = new List<Product>();
            var productsId = new List<int>();
            foreach (var shopReceipt in shop.Receipts)
            {
                foreach (var shopReceiptReceiptProduct in shopReceipt.ReceiptProducts)
                {
                    if (!productsId.Contains(shopReceiptReceiptProduct.ProductID))
                    {
                        productsId.Add(shopReceiptReceiptProduct.ProductID);
                        products.Add(shopReceiptReceiptProduct.Product);
                    }
                }
            }
            return products;
        }
    }

    public interface IShopHelper
    {
        List<Product> GetShopProducts(int shopId, int userId);
    }
}
