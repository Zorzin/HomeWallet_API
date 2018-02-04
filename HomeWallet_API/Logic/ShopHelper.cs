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

        public async Task<List<Product>> GetShopProducts(int shopId, int userId)
        {
            var products = await _context.ReceiptProducts
                .Include(rp => rp.Receipt)
                .Include(rp => rp.Product)
                .Where(rp => rp.Receipt.ShopID == shopId && rp.Receipt.UserID == userId)
                .Select(rp => rp.Product)
                .Distinct()
                .ToListAsync();
            return products;
        }
    }

    public interface IShopHelper
    {
        Task<List<Product>> GetShopProducts(int shopId, int userId);
    }
}
