using Microsoft.AspNetCore.Mvc;
using Bookstore.Data;
using Bookstore.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderHistoryController : ControllerBase
    {
        private readonly BookstoreDbContext _context;

        public OrderHistoryController(BookstoreDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderHistories
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderHistory>>> GetOrderHistories()
        {
            var OrderHistories = await _context.OrderHistories.Include(o => o.Book).Include(o => o.Customer).Select(o=> new{
            OrderId = o.Id,
            CustomerId = o.Customer.Id,
            CustomerName = o.Customer.Name,
            CustomerUsername = o.Customer.Username,
            CustomerEmail = o.Customer.Email,
            BookId = o.Book.Id,
            BookTitle = o.Book.Title,
            BookISBN = o.Book.ISBN,
            BookPrice = o.Book.Price,
            OrderDate = o.OrderDate,
            Quantity = o.Quantity,
            TotalPrice = o.TotalPrice,
            OrderStatus = o.OrderStatus,
            PaymentMethod = o.PaymentMethod,
            ShippingAddress = o.ShippingAddress,
            BillingAddress = o.BillingAddress,
            DeliveryDate = o.DeliveryDate
            }).ToListAsync();
            //var order = "select * from OrderHistories inner join Customer on OrderHistories.Id = Customer.Id inner join Book on Book.Id = OrderHistories.Id";
            return Ok(OrderHistories);
        }

        // GET: api/OrderHistories/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<OrderHistory>> GetOrderHistory(int id)
        {
            var orderHistory = await _context.OrderHistories.FindAsync(id);

            if (orderHistory == null)
            {
                return NotFound();
            }

            return orderHistory;
        }

        // PUT: api/OrderHistories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutOrderHistory(int id, OrderHistory orderHistory)
        {
            if (id != orderHistory.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHistoryExists(id))
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

        // POST: api/OrderHistories
        [HttpPost]
        [Authorize(Policy = "AllowCustomer")]
        public async Task<ActionResult<OrderHistory>> PostOrderHistory(OrderHistory orderHistory)
        {
            if(_context.Books.Any(x=>x.Id == orderHistory.BookID) && _context.Customers.Any(x=>x.Id == orderHistory.CustomerID))
            {
                _context.OrderHistories.Add(orderHistory);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetOrderHistory", new { id = orderHistory.Id }, orderHistory);
            }
            return BadRequest();
        }

        // DELETE: api/OrderHistories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderHistory(int id)
        {
            var orderHistory = await _context.OrderHistories.FindAsync(id);
            if (orderHistory == null)
            {
                return NotFound();
            }

            _context.OrderHistories.Remove(orderHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderHistoryExists(int id)
        {
            return _context.OrderHistories.Any(e => e.Id == id);
        }
    }
}
