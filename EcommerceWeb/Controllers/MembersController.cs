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
    public class MembersController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public MembersController(ShopDBContext context)
        {
            _context = context;
        }

        // GET: api/Members/GetMemberCount
        [HttpGet("GetMemberCount")]
        public int GetMemberCount()
        {
            return _context.Members.Count();
        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            return await _context.Members.ToListAsync();
        }

        // GET: api/Members/GetMembersByPagination?PageNumber=1&PageSize=6
        [HttpGet("GetMembersByPagination")]
        public async Task<ActionResult<PaginationFilter<Member>>> GetMembersByPagination([FromQuery] PaginationFilter<Member> filter, string orderBy)
        {
            var members = from m in _context.Members
                             select m;

            orderBy = String.IsNullOrEmpty(orderBy) ? "created_at" : orderBy;
            switch (orderBy)
            {
                case "created_at":
                    members = members.OrderByDescending(p => p.CreatedAt);
                    break;
                case "updated_at":
                    members = members.OrderByDescending(p => p.UpdatedAt);
                    break;
            }
            var pagedData = await members.Skip((filter.PageNumber - 1) * filter.PageSize)
                                        .Take(filter.PageSize)
                                        .ToListAsync();

            var totalPages = (int)Math.Ceiling(members.Count() / (double)filter.PageSize);

            return new PaginationFilter<Member>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                PagedData = pagedData,
                TotalPages = totalPages
            };
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // GET: api/Members/GetMemberByUsername/abc123
        [HttpGet("GetMemberByUsername/{username}")]
        public async Task<ActionResult<Member>> GetMemberByUsername(string username)
        {
            var member = await _context.Members.Where(m => m.Username == username).SingleAsync();

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // GET: api/Members/GetMemberByEmail/abc123
        [HttpGet("GetMemberByEmail/{email}")]
        public async Task<ActionResult<Member>> GetMemberByEmail(string email)
        {
            var member = await _context.Members.Where(m => m.Email == email).SingleAsync();

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // GET: api/Members/GetMemberStatusByEmail/abc123
        [HttpGet("GetMemberStatusByEmail/{email}")]
        public async Task<ActionResult<bool>> GetMemberStatusByEmail(string email)
        {
            var member = await _context.Members.Where(m => m.Email == email).SingleAsync();

            if (member == null)
            {
                return NotFound();
            }

            return member.IsActive;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.ID)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMember", new { id = member.ID }, member);
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.ID == id);
        }
    }
}
