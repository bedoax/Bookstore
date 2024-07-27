using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bookstore.Data;
using Bookstore.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly BookstoreDbContext _context;
        private readonly ILogger<OrderDetailsController> _logger;

        public OrderDetailsController(BookstoreDbContext context, ILogger<OrderDetailsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/OrderDetails
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            _logger.LogInformation("Getting all order details.");
            var orderDetails = await _context.OrderDetails.ToListAsync();
            if (!orderDetails.Any())
            {
                _logger.LogWarning("No order details found.");
                return NoContent();
            }
            return orderDetails;
        }

        // GET: api/OrderDetails/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        {
            _logger.LogInformation("Getting order detail with ID: {Id}", id);
            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                _logger.LogWarning("Order detail with ID: {Id} not found.", id);
                return NotFound();
            }

            return orderDetail;
        }

        // PUT: api/OrderDetails/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutOrderDetail(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                _logger.LogWarning("Mismatch between route ID: {RouteId} and order detail ID: {OrderDetailId}.", id, orderDetail.Id);
                return BadRequest();
            }

            _context.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order detail with ID: {Id} updated successfully.", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
                {
                    _logger.LogWarning("Order detail with ID: {Id} not found during update.", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency exception while updating order detail with ID: {Id}.", id);
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/OrderDetails/5
        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }*/

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.Id == id);
        }
    }
}
