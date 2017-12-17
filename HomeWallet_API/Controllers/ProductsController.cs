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
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private readonly DBContext _context;
        private IProductHelper _productHelper;

        public ProductsController(DBContext context, IProductHelper productHelper)
        {
            _context = context;
            _productHelper = productHelper;
        }

        // GET: api/Products/1
        [HttpGet("{userId}")]
        public IEnumerable<Product> GetProducts(int userId)
        {
            return _context.Products.Where(p=>p.UserID == userId);
        }

        // GET: api/Products/5
        [HttpGet("{userId}/{id}")]
        public async Task<IActionResult> GetProduct(int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.ID == id && m.UserID == userId);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: api/Products/categories/1/5
        [HttpGet("categories/{userId}/{id}")]
        public async Task<IActionResult> GetProductCategories(int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categories = _productHelper.GetCategories(userId,id);

            if (categories == null)
            {
                return NotFound();
            }

            return Ok(categories);
        }

        // PUT: api/Products/1/5
        [HttpPut("{userId}/{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int userId, [FromRoute] int id, [FromBody] ProductPost product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _productHelper.UpdateProduct(userId,id,product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products/1
        [HttpPost("{userId}")]
        public async Task<int> PostProduct(int userId, [FromBody] ProductPost postProduct)
        {
            var product = new Product
            {
                Name = postProduct.name,
                UserID = userId
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            foreach (var category in postProduct.categories)
            {
                var productCategory = new ProductCategory
                {
                    CategoryID = category,
                    ProductID = product.ID
                };
                _context.ProductCategories.Add(productCategory);
            }
            await _context.SaveChangesAsync();


            return product.ID;
        }

        // DELETE: api/Products/5
        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteProduct(int userId,[FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.ID == id && m.UserID == userId);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}