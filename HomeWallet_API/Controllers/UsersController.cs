﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeWallet_API.Models;
using HomeWallet_API.Models.POST;
using Microsoft.AspNetCore.Identity;

namespace HomeWallet_API.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly DBContext _context;
        public UsersController(DBContext context)
        {
            _context = context;
        }
        // GET: api/Users/
        [HttpGet()]
        public async Task<IActionResult> GetLogins()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUserLogins = _context.Users.Select(u => u.Nick);

            if (!dbUserLogins.Any())
            {
                return BadRequest(ModelState);
            }

            return Ok(dbUserLogins);
        }

        // GET: api/Users/emails
        [HttpGet("emails")]
        public async Task<IActionResult> GetEmails()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUserEmails = _context.Users.Select(u=>u.Email);

            if (!dbUserEmails.Any())
            {
                return BadRequest(ModelState);
            }

            return Ok(dbUserEmails);
        }
        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegister user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user.Password != user.RePassword)
            {
                return BadRequest(ModelState);
            }

            if (_context.Users.Any(u => u.Email == user.Email || u.Nick == user.Login))
            {
                return BadRequest(ModelState);
            }

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var dbUser = new User()
            {
                Currency = user.Currency,
                Email = user.Email,
                Nick = user.Login,
                Theme = user.Theme,
                Language = user.Language
            };
            dbUser.Password = hasher.HashPassword(dbUser, user.Password);
            _context.Users.Add(dbUser);
            await _context.SaveChangesAsync();

            return Ok(dbUser.Id);
        }

        // GET: api/Users/currency
        [HttpGet("currency/{userId}")]
        public async Task<IActionResult> GetCurrency([FromRoute] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }

            return Ok(dbUser.Currency);
        }

        // GET: api/Users/theme
        [HttpGet("theme/{userId}")]
        public async Task<IActionResult> GetTheme([FromRoute] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }

            return Ok(dbUser.Theme);
        }

        // GET: api/Users/language
        [HttpGet("language/{userId}")]
        public async Task<IActionResult> GetLanguage([FromRoute] int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }

            return Ok(dbUser.Language);
        }

        // Post: api/Users/currency
        [HttpPost("currency/{userId}/{value}")]
        public async Task<IActionResult> SetCurrency([FromRoute] int userId,[FromRoute] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }

            dbUser.Currency = value;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Post: api/Users/language
        [HttpPost("language/{userId}/{value}")]
        public async Task<IActionResult> SetLanguage([FromRoute] int userId, [FromRoute] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }

            dbUser.Language = value;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Post: api/Users/theme
        [HttpPost("theme/{userId}/{value}")]
        public async Task<IActionResult> SetTheme([FromRoute] int userId, [FromRoute] string value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }

            dbUser.Theme = value;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Nick == user.Login);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(dbUser, dbUser.Password, user.Password);
            if (result == PasswordVerificationResult.Success)
            {
                return Ok(dbUser.Id);
            }

            return BadRequest(ModelState);

        }

        // POST: api/Users/password
        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserPassword user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user.NewPassword != user.ReNewPassword)
            {
                return BadRequest(ModelState);
            }

            var dbUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(dbUser, dbUser.Password, user.OldPassword);
            if (result != PasswordVerificationResult.Success)
            {
                return BadRequest(ModelState);
            }

            dbUser.Password = hasher.HashPassword(dbUser,user.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(dbUser.Id);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}