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
using Microsoft.Azure.KeyVault.Models;

namespace HomeWallet_API.Controllers
{
    [Produces("application/json")]
    [Route("api/Plans")]
    public class PlansController : Controller
    {
        private readonly DBContext _context;
        private readonly IPlanHelper _planHelper;
        private readonly IPlanSummaryHelper _planSummaryHelper;

        public PlansController(DBContext context, IPlanHelper planHelper, IPlanSummaryHelper planSummaryHelper)
        {
            _context = context;
            _planHelper = planHelper;
            _planSummaryHelper = planSummaryHelper;
        }

        // GET: api/Plans/1
        [HttpGet("{userId}")]
        public IEnumerable<Plan> GetPlans([FromRoute] int userId)
        {
            return _context.Plans.Where(p=>p.UserID == userId).OrderByDescending(p=>p.StartDate);
        }

        // GET: api/plans/summary/
        [HttpGet("summary/{userId}/{planId}")]
        public async Task<IActionResult> GetSummaryByDate(int userId, int planId)
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

            var plan = _context.Plans.FirstOrDefault(u => u.ID == planId && u.UserID == userId);

            if (plan == null)
            {
                return BadRequest();
            }

            var summary = await _planSummaryHelper.GetPlanSummary(userId, planId);

            if (summary == null)
            {
                return BadRequest();
            }

            return Ok(summary);
        }
        // GET: api/Plans/1/2
        [HttpGet("details/{userId}/{id}")]
        public async Task<IActionResult> GetPlanWithDetails([FromRoute] int userId, [FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plan = await _planHelper.GetPlan(userId, id);
            var planDetails = _planHelper.GetPlanDetails(plan);

            if (planDetails == null)
            {
                return NotFound();
            }

            return Ok(planDetails);
        }

        // GET: api/Plans/1/19-08-2017
        [HttpGet("{userId}/{date}")]
        public async Task<IActionResult> GetPlanForDate([FromRoute] int userId, [FromRoute]string date)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plan = await _planHelper.GetPlanForDate(userId,date);
            if (plan == null)
            {
                return Ok(null);
            }

            return Ok(plan);
        }

        // GET: api/Plans/id/1/2
        [HttpGet("id/{userId}/{id}")]
        public async Task<IActionResult> GetPlanById([FromRoute] int userId, [FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plan = await _planHelper.GetPlan(userId, id);
            if (plan == null)
            {
                return Ok(null);
            }

            return Ok(plan);
        }

        // PUT: api/Plans/1/5
        [HttpPut("{userId}/{id}")]
        public async Task<IActionResult> PutPlan([FromRoute] int userId, [FromRoute] int id, [FromBody] PlanEdit planEdit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentPlan = _planHelper.GetPlanForDate(userId, DateTime.Today.ToString());
            var plan = _planHelper.GetPlan(userId, id).Result;

            if (plan == null)
            {
                return NotFound();
            }

            if (id != plan.ID || userId != plan.UserID || plan.ID != currentPlan.Result.ID)
            {
                return BadRequest();
            }

            plan.Amount = planEdit.Amount;
            plan.StartDate = DateTime.Parse(planEdit.StartDate);
            plan.EndDate = DateTime.Parse(planEdit.EndDate);

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

            try
            {
                var plan = new Plan()
                {
                    Amount = planPost.Amount,
                    EndDate = DateTime.Parse(planPost.EndDate),
                    StartDate = DateTime.Parse(planPost.StartDate),
                    UserID = userId
                };

                _context.Plans.Add(plan);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest();
            }

            return Ok();
        }

        // DELETE: api/Plans/1/5
        [HttpDelete("{userId}/{id}")]
        public async Task<IActionResult> DeletePlan([FromRoute] int userId,[FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plan = await _context.Plans.SingleOrDefaultAsync(m => m.ID == id && m.UserID == userId);
            if (plan == null)
            {
                return NotFound();
            }

            var currentPlan = _planHelper.GetPlanForDate(userId, DateTime.Today.ToString());

            if (plan.ID != currentPlan.Result.ID)
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