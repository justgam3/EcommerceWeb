using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using EcommerceWebApi.Filter;

namespace EcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public OrdersController(ShopDBContext context)
        {
            _context = context;
        }

        // GET: api/Products/GetOrderCount
        [HttpGet("GetOrderCount")]
        public int GetOrderCount()
        {
            return _context.Orders.Count();
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/GetOrdersByPagination?PageNumber=1&PageSize=6
        [HttpGet("GetOrdersByPagination")]
        public async Task<ActionResult<PaginationFilter<Order>>> GetOrdersByPagination([FromQuery] PaginationFilter<Order> filter, string orderBy)
        {
            var orders = from o in _context.Orders
                           select o;

            orderBy = String.IsNullOrEmpty(orderBy) ? "created_at" : orderBy;
            switch (orderBy)
            {
                case "created_at":
                    orders = orders.OrderByDescending(p => p.OrderDate);
                    break;
                case "updated_at":
                    orders = orders.OrderByDescending(p => p.UpdatedAt);
                    break;
            }
            var pagedData = await orders.Skip((filter.PageNumber - 1) * filter.PageSize)
                                        .Take(filter.PageSize)
                                        .ToListAsync();

            var totalPages = (int)Math.Ceiling(orders.Count() / (double)filter.PageSize);

            return new PaginationFilter<Order>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                PagedData = pagedData,
                TotalPages = totalPages
            };
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // GET: api/Orders/5/GetAdminOrderDetails
        [HttpGet("{id}/GetAdminOrderDetails")]
        public async Task<ActionResult<Order>> GetAdminOrderDetails(int id)
        {
            var order = await _context.Orders.SingleAsync(o => o.ID == id);

            await _context.Entry(order)
                .Reference(o => o.Member)
                .LoadAsync();

            await _context.Entry(order)
                .Collection(o => o.OrderDetails)
                .Query()
                .Include(o => o.Variant)
                .ThenInclude(o => o.Product)
                .ThenInclude(o => o.ProductImages)
                .LoadAsync();

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // GET: api/Orders/kenng123/GetOrderByUsername
        [HttpGet("{username}/GetOrderByUsername")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrderByUsername(string username)
        {
            var orders = await _context.Orders.Where(o => o.Member.Username == username).ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }

            return orders;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.ID)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.ID }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.ID == id);
        }
    }
}
