﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Logic;
using HomeWallet_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeWallet_API.Controllers
{
    [Produces("application/json")]
    [Route("api/Summary")]
    public class SummaryController : Controller
    {
        private ISummaryHelper _summaryHelper;
        private DBContext _dbContext;

        public SummaryController(DBContext dbContext, ISummaryHelper summaryHelper)
        {
            _summaryHelper = summaryHelper;
            _dbContext = dbContext;
        }

        // GET: api/summary/daily/
        [HttpGet("daily/{userId}/{date}")]
        public async Task<IActionResult> GetDailySummaryByDate(int userId,string date)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return BadRequest();
            }

            var dailySummary = _summaryHelper.GetDailySummary(userId, date);

            if (dailySummary == null)
            {
                return BadRequest();
            }

            return Ok(dailySummary);
        }

    }
}