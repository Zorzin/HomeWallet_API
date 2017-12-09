using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeWallet_API.Models;
using HomeWallet_API.Models.POST;

namespace HomeWallet_API.Controllers
{
    [Produces("application/json")]
    [Route("api/Plans")]
    public class PlansController : Controller
    {
        private readonly DBContext _context;

        public PlansController(DBContext context)
        {
            _context = context;
        }

        // GET: api/Plans
        [HttpGet]
        public IEnumerable<Plan> GetPlans()
        {
            return _context.Plans;
        }

        // GET: api/Plans/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlan([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plan = await _context.Plans.SingleOrDefaultAsync(m => m.ID == id);

            if (plan == null)
            {
                return NotFound();
            }

            return Ok(plan);
        }

        // PUT: api/Plans/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlan([FromRoute] int id, [FromBody] Plan plan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != plan.ID)
            {
                return BadRequest();
            }

            _context.Entry(plan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(id))
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

        // POST: api/Plans/1
        [HttpPost("{userId}")]
        public async Task<IActionResult> PostPlan([FromRoute] int userId, [FromBody] PlanPost planPost)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plan = new Plan()
            {
                Amount = planPost.Amount,
                EndDate = DateTime.Parse(planPost.StartDate),
                StartDate = DateTime.Parse(planPost.EndDate),
                UserID = userId
            };

            _context.Plans.Add(plan);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Plans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plan = await _context.Plans.SingleOrDefaultAsync(m => m.ID == id);
            if (plan == null)
            {
                return NotFound();
            }

            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();

            return Ok(plan);
        }

        private bool PlanExists(int id)
        {
            return _context.Plans.Any(e => e.ID == id);
        }
    }
}