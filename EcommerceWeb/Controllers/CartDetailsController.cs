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
    public class CartDetailsController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public CartDetailsController(ShopDBContext context)
        {
            _context = context;
        }

        // GET: api/CartDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartDetail>>> GetCartDetails()
        {
            return await _context.CartDetails.ToListAsync();
        }

        //// GET: api/CartDetails/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<CartDetail>> GetCartDetail(int id)
        //{
        //    var cartDetail = await _context.CartDetails.FindAsync(id);

        //    if (cartDetail == null)
        //    {
        //        return NotFound();
        //    }

        //    return cartDetail;
        //}

        // GET: api/CartDetails/5/GetCartDetailById
        [HttpGet("{id}/GetCartDetailById")]
        public async Task<ActionResult<UserCartDetailDTO>> GetCartDetailById(int id)
        {
            var memberCart = await _context.Members.SingleAsync(m => m.ID == id);

            await _context.Entry(memberCart)
                .Collection(m => m.CartDetails)
                .Query()
                .Where(m => m.MemberID == id)
                .Include(m => m.Variant)
                .ThenInclude(m => m.Product)
                .ThenInclude(m => m.ProductImages)
                .LoadAsync();

            if (memberCart == null)
            {
                return NotFound();
            }

            return new UserCartDetailDTO
            {
                MemberID = memberCart.ID,
                CartDetails = memberCart.CartDetails,
                TotalPrice = memberCart.CartDetails.Sum(cd => cd.Quantity * cd.Variant.Product.Price)
            };
        }

        // PUT: api/CartDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartDetail(int id, CartDetail cartDetail)
        {
            if (id != cartDetail.MemberID)
            {
                return BadRequest();
            }

            _context.Entry(cartDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartDetailExists(id))
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

        // POST: api/CartDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CartDetail>> PostCartDetail(CartDetail cartDetail)
        {
            _context.CartDetails.Add(cartDetail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartDetailExists(cartDetail.MemberID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCartDetail", new { id = cartDetail.MemberID }, cartDetail);
        }

        // DELETE: api/CartDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartDetail(int id)
        {
            var cartDetail = await _context.CartDetails.FindAsync(id);
            if (cartDetail == null)
            {
                return NotFound();
            }

            _context.CartDetails.Remove(cartDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartDetailExists(int id)
        {
            return _context.CartDetails.Any(e => e.MemberID == id);
        }
    }
}
