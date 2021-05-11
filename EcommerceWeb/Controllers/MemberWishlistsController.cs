using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;

namespace EcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberWishlistsController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public MemberWishlistsController(ShopDBContext context)
        {
            _context = context;
        }

        // GET: api/MemberWishlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberWishlist>>> GetMemberWishlists()
        {
            return await _context.MemberWishlists.ToListAsync();
        }

        // GET: api/MemberWishlists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberWishlist>> GetMemberWishlist(int id)
        {
            var memberWishlist = await _context.MemberWishlists.FindAsync(id);

            if (memberWishlist == null)
            {
                return NotFound();
            }

            return memberWishlist;
        }

        // PUT: api/MemberWishlists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMemberWishlist(int id, MemberWishlist memberWishlist)
        {
            if (id != memberWishlist.MemberID)
            {
                return BadRequest();
            }

            _context.Entry(memberWishlist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberWishlistExists(id))
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

        // POST: api/MemberWishlists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MemberWishlist>> PostMemberWishlist(MemberWishlist memberWishlist)
        {
            _context.MemberWishlists.Add(memberWishlist);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MemberWishlistExists(memberWishlist.MemberID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMemberWishlist", new { id = memberWishlist.MemberID }, memberWishlist);
        }

        // DELETE: api/MemberWishlists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMemberWishlist(int id)
        {
            var memberWishlist = await _context.MemberWishlists.FindAsync(id);
            if (memberWishlist == null)
            {
                return NotFound();
            }

            _context.MemberWishlists.Remove(memberWishlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberWishlistExists(int id)
        {
            return _context.MemberWishlists.Any(e => e.MemberID == id);
        }
    }
}
