using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using FluentValidation;

namespace EcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariantsController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public VariantsController(ShopDBContext context)
        {
            _context = context;
        }

        // GET: api/Variants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Variant>>> GetVariants()
        {
            return await _context.Variants.ToListAsync();
        }

        // GET: api/Variants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Variant>> GetVariant(int id)
        {
            var variant = await _context.Variants.FindAsync(id);

            if (variant == null)
            {
                return NotFound();
            }

            return variant;
        }

        // PUT: api/Variants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVariant(int id, Variant variant)
        {
            if (id != variant.ID)
            {
                return BadRequest();
            }

            _context.Entry(variant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VariantExists(id))
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

        // POST: api/Variants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Variant>> PostVariant(Variant variant)
        {
            if(TypeExists(variant.Type, variant.ProductID))
            {
                return Conflict();
            }
            _context.Variants.Add(variant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVariant", new { id = variant.ID }, variant);
        }

        // DELETE: api/Variants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int id)
        {
            var variant = await _context.Variants.FindAsync(id);
            if (variant == null)
            {
                return NotFound();
            }

            _context.Variants.Remove(variant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VariantExists(int id)
        {
            return _context.Variants.Any(e => e.ID == id);
        }

        private bool TypeExists(string variant_type, int product_id)
        {
            return _context.Variants.Any(e => e.Type == variant_type && e.ProductID == product_id);
        }
    }
}
