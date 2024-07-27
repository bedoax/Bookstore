using Bookstore.Data;
using Bookstore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(BookstoreDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            _logger.LogInformation("Fetching all admins");
            var admins = await _context.Admins.ToListAsync();
            _logger.LogInformation("{Count} admins found", admins.Count);
            return admins;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            _logger.LogInformation("Fetching admin with id {Id}", id);
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null)
            {
                _logger.LogWarning("Admin with id {Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Admin with id {Id} found", id);
            return admin;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<Admin>> PostAdmin(Admin admin)
        {
            _logger.LogInformation("Creating a new admin with username {Username}", admin.Username);

            if (_context.Admins.Any(x => x.Username == admin.Username && x.Password == admin.Password))
            {
                _logger.LogWarning("Admin with username {Username} already exists", admin.Username);
                return BadRequest();
            }

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin with username {Username} created successfully", admin.Username);
            return CreatedAtAction(nameof(GetAdmin), new { id = admin.Id }, admin);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> PutAdmin(int id, Admin admin)
        {
            if (id != admin.Id)
            {
                _logger.LogWarning("Admin id {Id} in the URL does not match admin id {AdminId} in the request body", id, admin.Id);
                return BadRequest();
            }

            _logger.LogInformation("Updating admin with id {Id}", id);
            _context.Entry(admin).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin with id {Id} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            _logger.LogInformation("Deleting admin with id {Id}", id);
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                _logger.LogWarning("Admin with id {Id} not found", id);
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Admin with id {Id} deleted successfully", id);
            return NoContent();
        }
    }
}
