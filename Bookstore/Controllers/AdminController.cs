using Bookstore.Data;
using Bookstore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly BookstoreDbContext _context;

        public AdminController(BookstoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            return await _context.Admins.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
            if(_context.Admins.Any(x=>x.Username == admin.Username && x.Password == admin.Password))
            {
                return BadRequest();
            }
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdmin), new { id = admin.Id }, admin);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> PutAdmin(int id, Admin admin)
        {
            if (id != admin.Id)
            {
                return BadRequest();
            }

            _context.Entry(admin).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
