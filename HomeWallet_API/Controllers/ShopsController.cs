using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ShopsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Shops/1
        [HttpGet("{userId}")]
        public IEnumerable<Shop> GetShops(int userId)
        {
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

        // PUT: api/Shops/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShop([FromRoute] int id, [FromBody] Shop shop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shop.ID)
            {
                return BadRequest();
            }

            _context.Entry(shop).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Shops
        [HttpPost("{userId}")]
        public async Task PostShop(int userId, [FromBody] string name)
        {
            Console.WriteLine("New name: "+name);
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var shop = _context.Shops.Add(new Shop(name,userId));
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetShop", userId,new {id = shop.Entity.ID });
        }

        // DELETE: api/Shops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shop = await _context.Shops.SingleOrDefaultAsync(m => m.ID == id);
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