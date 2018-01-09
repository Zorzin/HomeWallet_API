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
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly DBContext _context;

        public UsersController(DBContext context)
        {
            _context = context;
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

            var dbUser = new User()
            {
                Currency = user.Currency,
                Email = user.Email,
                Nick = user.Login,
                Password = user.Password,
                Theme = user.Theme,
                Language = user.Language
            };
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

            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Nick == user.Login && u.Password == user.Password);

            if (dbUser == null)
            {
                return BadRequest(ModelState);
            }

            return Ok(dbUser.Id);
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

            if (dbUser.Password != user.OldPassword)
            {
                return BadRequest(ModelState);
            }

            dbUser.Password = user.NewPassword;

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