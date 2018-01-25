using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeWallet_API.Models;

namespace HomeWallet_API.Controllers
{
    [Produces("application/json")]
    [Route("api/Shops")]
    public class ShopsController : Controller
    {
        private readonly DBContext _context;
        private readonly IShopHelper _shopHelper;
        private readonly IShopSummaryHelper _shopSummaryHelper;

        public ShopsController(DBContext context, IShopHelper shopHelper, IShopSummaryHelper shopSummaryHelper)
        {
            _context = context;
            _shopHelper = shopHelper;
            _shopSummaryHelper = shopSummaryHelper;
        }

        // GET: api/Shops/1
        [HttpGet("{userId}")]
        public IEnumerable<Shop> GetShops(int userId)
        {
            _shopSummaryHelper.GetShopSummary(1, 39, "2017-01-01", "2019-01-01");
            return _context.Shops.Where(s=>s.UserID == userId);
        }

        // GET: api/Shops/1/4
        [HttpGet("{userId}/{id}")]
        public async Task<IActionResult> GetShop(int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shop = await _context.Shops.SingleOrDefaultAsync(m => m.ID == id && m.UserID == userId);

            if (shop == null)
            {
                return NotFound();
            }

            return Ok(shop);
        }

        // GET: api/Shops/products/1/4
        [HttpGet("products/{userId}/{id}")]
        public async Task<IActionResult> GetShopProducts(int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = _shopHelper.GetShopProducts(id, userId);
            
            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        // GET: api/shops/summary/
        [HttpGet("summary/{userId}/{shopId}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetSummaryByDate(int userId, int shopId, string startDate, string endDate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return BadRequest();
            }

            var shop = _context.Shops.FirstOrDefault(u => u.ID == shopId && u.UserID == userId);

            if (shop == null)
            {
                return BadRequest();
            }

            var summary = await _shopSummaryHelper.GetShopSummary(userId, shopId, startDate, endDate);

            if (summary == null)
            {
                return BadRequest();
            }

            return Ok(summary);
        }

        // PUT: api/Shops/1/5/sklep
        [HttpPut("{userId}/{id}/{name}")]
        public async Task<IActionResult> PutShop(int userId, [FromRoute] int id, string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shop = _context.Shops.FirstOrDefault(s => s.UserID == userId && s.ID == id);
            shop.Name = name;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Shops
        [HttpPost("{userId}")]
        public async Task<IActionResult> PostShop(int userId, [FromBody] string name)
        {
            var shop = new Shop(name, userId);
            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();
            return Ok(shop.ID);
        }

        // DELETE: api/Shops/1/5
        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteShop(int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shop = await _context.Shops.SingleOrDefaultAsync(m => m.ID == id && m.UserID == userId);
            if (shop == null)
            {
                return NotFound();
            }

            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();

            return Ok(shop);
        }

        private bool ShopExists(int id)
        {
            return _context.Shops.Any(e => e.ID == id);
        }
    }
}