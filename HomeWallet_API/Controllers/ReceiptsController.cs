using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeWallet_API.Models;
using HomeWallet_API.Models.POST;
using HomeWallet_API.Models.PUT;

namespace HomeWallet_API.Controllers
{
    [Produces("application/json")]
    [Route("api/Receipts")]
    public class ReceiptsController : Controller
    {
        private readonly DBContext _context;
        private readonly IReceiptHelper _receiptHelper;
        private readonly IProductHelper _productHelper;

        public ReceiptsController(DBContext context, IReceiptHelper receiptHelper, IProductHelper productHelper)
        {
            _context = context;
            _receiptHelper = receiptHelper;
            _productHelper = productHelper;
        }


        // GET: api/Receipts/userId/totalValue/id
        [HttpGet("{userId}/totalValue/{id}")]
        public double GetReceiptTotalValue(int userId, int id)
        {
            var receipt = _context.Receipts.Include(r=>r.ReceiptProducts).SingleOrDefaultAsync(r => r.UserID == userId && r.ID == id);
            double total=0;

            foreach (var receiptProduct in receipt.Result.ReceiptProducts)
            {
                total += (receiptProduct.Price * receiptProduct.Amount);
            }
            return total;
        }

        // GET: api/Receipts/userid
        [HttpGet("{userId}")]
        public IEnumerable<Receipt> GetReceipts(int userId)
        {
            return _context.Receipts.Where(r=>r.UserID == userId).OrderByDescending(r=>r.PurchaseDate);
        }

        // GET: api/Receipts/userid/5
        [HttpGet("{userId}/{id}")]
        public async Task<IActionResult> GetReceipt(int userId,[FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var receipt = await _context.Receipts.Where(r => r.UserID == userId).SingleOrDefaultAsync(m => m.ID == id);

            if (receipt == null)
            {
                return NotFound();
            }

            return Ok(receipt);
        }

        // PUT: api/Receipts/5
        [HttpPut("{userId}/{id}")]
        public async Task<IActionResult> PutReceipt([FromRoute] int userId, [FromRoute] int id, [FromBody] ReceiptEdit receiptEdit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _receiptHelper.UpdateReceipt(userId,id,receiptEdit);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceiptExists(id))
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

        // POST: api/Receipts
        [HttpPost("{userId}")]
        public async Task<IActionResult> PostReceipt(int userId, [FromBody] ReceiptPost receipt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tempReceipt = _productHelper.CreateReceipt(receipt.ShopId, userId, DateTime.Parse(receipt.Date));
            _productHelper.CreateReceiptProducts(receipt.Products, tempReceipt.ID);

            return Ok();
        }

        // POST: api/Receipts
        [HttpPost("cyclical/{userId}")]
        public async Task<IActionResult> PostReceiptCyclical(int userId, [FromBody] ReceiptCyclicalPost receipt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var start =DateTime.Parse(receipt.StartDate);
            while (start <= DateTime.Parse(receipt.EndDate))
            {
                var tempReceipt = _productHelper.CreateReceipt(receipt.ShopId, userId, start);
                _productHelper.CreateReceiptProducts(receipt.Products, tempReceipt.ID);
                start = start.AddDays(receipt.Cycle);
            }

            return Ok();
        }

        // DELETE: api/Receipts/1/5
        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeleteReceipt(int userId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var receipt = await _context.Receipts.SingleOrDefaultAsync(m => m.ID == id && m.UserID==userId);
            if (receipt == null)
            {
                return NotFound();
            }

            _context.Receipts.Remove(receipt);
            await _context.SaveChangesAsync();

            return Ok(receipt);
        }

        private bool ReceiptExists(int id)
        {
            return _context.Receipts.Any(e => e.ID == id);
        }
    }
}