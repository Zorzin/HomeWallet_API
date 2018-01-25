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
    [Route("api/Categories")]
    public class CategoriesController : Controller
    {
        private readonly DBContext _context;
        private readonly ICategoryHelper _categoryHelper;
        private readonly ICategorySummaryHelper _categorySummaryHelper;

        public CategoriesController(DBContext context, ICategoryHelper categoryHelper, ICategorySummaryHelper categorySummaryHelper)
        {
            _context = context;
            _categoryHelper = categoryHelper;
            _categorySummaryHelper = categorySummaryHelper;
        }

        // GET: api/Categories
        [HttpGet("{userId}")]
        public IEnumerable<Category> GetCategories(int userId)
        {
            return _context.Categories.Where(c=>c.UserID == userId);
        }

        // GET: api/Categories/5/1
        [HttpGet("{userId}/{id}")]
        public async Task<IActionResult> GetCategory(int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.SingleOrDefaultAsync(m => m.ID == id && m.UserID == userId);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // GET: api/Categories/products/5/1
        [HttpGet("products/{userId}/{id}")]
        public async Task<IActionResult> GetCategoryProducts(int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = _categoryHelper.GetCategoryProducts(id, userId);

            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        // GET: api/categories/summary/
        [HttpGet("summary/{userId}/{categoryId}/{startDate}/{endDate}")]
        public async Task<IActionResult> GetSummaryByDate(int userId, int categoryId, string startDate, string endDate)
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

            var category = _context.Categories.FirstOrDefault(u => u.ID == categoryId && u.UserID == userId);

            if (category == null)
            {
                return BadRequest();
            }

            var summary = await _categorySummaryHelper.GetCategorySummary(userId, categoryId, startDate, endDate);

            if (summary == null)
            {
                return BadRequest();
            }

            return Ok(summary);
        }

        // PUT: api/Categories/1/5/name
        [HttpPut("{userId}/{id}/{name}")]
        public async Task<IActionResult> PutCategory([FromRoute] int userId,[FromRoute] int id, [FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = _context.Categories.FirstOrDefault(c => c.UserID == userId && c.ID == id);
            category.Name = name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories/1/name
        [HttpPost("{userId}/{name}")]
        public async Task<IActionResult> PostCategory(int userId,string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = new Category()
            {
                Name = name,
                UserID = userId

            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // DELETE: api/Categories/1/5
        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int userId,[FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.SingleOrDefaultAsync(m => m.ID == id && m.UserID == userId);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.ID == id);
        }
    }
}