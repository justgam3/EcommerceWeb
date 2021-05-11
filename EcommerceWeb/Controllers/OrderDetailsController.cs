using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using EcommerceWebApi.DTO;

namespace EcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public OrderDetailsController(ShopDBContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            return await _context.OrderDetails.ToListAsync();
        }

        //// GET: api/OrderDetails/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<OrderDetail>> GetOrderDetail(int id)
        //{
        //    var orderDetail = await _context.OrderDetails.FindAsync(id);

        //    if (orderDetail == null)
        //    {
        //        return NotFound();
        //    }

        //    return orderDetail;
        //}

        // GET: api/OrderDetails/5/GetOrderDetailById
        [HttpGet("{id}/GetOrderDetailById")]
        public async Task<ActionResult<Order>> GetOrderDetailById(int id)
        {
            var order = await _context.Orders.SingleAsync(o => o.ID == id);

            await _context.Entry(order)
                .Collection(o => o.OrderDetails)
                .Query()
                .Where(o => o.OrderID == id)
                .Include(m => m.Variant)
                .ThenInclude(m => m.Product)
                .ThenInclude(m => m.ProductImages)
                .LoadAsync();

            if (order == null)
            {
                return NotFound();
            }

            return new Order
            {
                ID = order.ID,
                PaymentID = order.PaymentID,
                OrderDetails = order.OrderDetails,
                Country = order.Country,
                State = order.State,
                PostCode = order.PostCode,
                StatusDesc = order.StatusDesc,
                TotalPrice = order.OrderDetails.Sum(cd => cd.Quantity * cd.Variant.Product.Price)
            };
        }

        // PUT: api/OrderDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderID)
            {
                return BadRequest();
            }

            _context.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
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

        // POST: api/OrderDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderDetail>> PostOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderDetailExists(orderDetail.OrderID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderDetail", new { id = orderDetail.OrderID }, orderDetail);
        }

        // DELETE: api/OrderDetails/5
        [HttpDelete("{id}")]
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
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderID == id);
        }
    }
}
