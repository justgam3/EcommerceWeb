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
    public class CategoriesController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public CategoriesController(ShopDBContext context)
        {
            _context = context;
        }

        // GET: api/Products/GetCategoryCount
        [HttpGet("GetCategoryCount")]
        public int GetCategoryCount()
        {
            return _context.Categories.Count();
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories
                .Include(c => c.ProductCategories)
                .ToListAsync();
        }

        // GET: api/Categories/GetCategoriesByPagination?PageNumber=1&PageSize=6
        [HttpGet("GetCategoriesByPagination")]
        public async Task<ActionResult<PaginationFilter<Category>>> GetCategoriesByPagination([FromQuery] PaginationFilter<Category> filter, string orderBy)
        {
            var categories = from c in _context.Categories
                           select c;

            orderBy = String.IsNullOrEmpty(orderBy) ? "created_at" : orderBy;
            switch (orderBy)
            {
                case "created_at":
                    categories = categories.OrderByDescending(p => p.CreatedAt);
                    break;
                case "updated_at":
                    categories = categories.OrderByDescending(p => p.UpdatedAt);
                    break;
            }
            var pagedData = await categories.Skip((filter.PageNumber - 1) * filter.PageSize)
                                        .Take(filter.PageSize)
                                        .ToListAsync();

            var totalPages = (int)Math.Ceiling(categories.Count() / (double)filter.PageSize);

            return new PaginationFilter<Category>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                PagedData = pagedData,
                TotalPages = totalPages
            };
        }

        // GET: api/Categories/GetActiveCategories
        [HttpGet("GetActiveCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetActiveCategories()
        {
            return await _context.Categories.Where(c => c.IsActive).ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.ID)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.ID }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.ID == id);
        }

        private bool CategoryExists(string category_name)
        {
            return _context.Categories.Any(e => e.Name == category_name);
        }
    }
}
