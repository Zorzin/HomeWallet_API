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
    [Route("api/ReceiptProducts")]
    public class ReceiptProductsController : Controller
    {
        private readonly DBContext _context;

        public ReceiptProductsController(DBContext context)
        {
            _context = context;
        }

        // GET: api/ReceiptProducts/receipt/1
        [HttpGet("receipt/{receiptId}")]
        public IEnumerable<ReceiptProduct> GetReceiptProductsByReceipt(int receiptId)
        {
            return _context.ReceiptProducts.Where(r=>r.ReceiptID == receiptId);
        }


        // GET: api/ReceiptProducts/product/1
        [HttpGet("product/{receiptId}")]
        public IEnumerable<ReceiptProduct> GetReceiptProductsByProduct(int productId)
        {
            return _context.ReceiptProducts.Where(r => r.ProductID == productId);
        }

        // GET: api/ReceiptProducts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceiptProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var receiptProduct = await _context.ReceiptProducts.SingleOrDefaultAsync(m => m.ID == id);

            if (receiptProduct == null)
            {
                return NotFound();
            }

            return Ok(receiptProduct);
        }

        // PUT: api/ReceiptProducts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceiptProduct([FromRoute] int id, [FromBody] ReceiptProduct receiptProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != receiptProduct.ID)
            {
                return BadRequest();
            }

            _context.Entry(receiptProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceiptProductExists(id))
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

        // POST: api/ReceiptProducts
        [HttpPost]
        public async Task<IActionResult> PostReceiptProduct([FromBody] ReceiptProduct receiptProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ReceiptProducts.Add(receiptProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReceiptProduct", new { id = receiptProduct.ID }, receiptProduct);
        }

        // DELETE: api/ReceiptProducts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceiptProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var receiptProduct = await _context.ReceiptProducts.SingleOrDefaultAsync(m => m.ID == id);
            if (receiptProduct == null)
            {
                return NotFound();
            }

            _context.ReceiptProducts.Remove(receiptProduct);
            await _context.SaveChangesAsync();

            return Ok(receiptProduct);
        }

        private bool ReceiptProductExists(int id)
        {
            return _context.ReceiptProducts.Any(e => e.ID == id);
        }
    }
}