using EcommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public ActionController(ShopDBContext context)
        {
            _context = context;
        }

        // PUT: api/Action/UpdateCart/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateCart/{id}")]
        public async Task<IActionResult> UpdateCart(int id, CartDetail cartDetail)
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

            _context.Entry(cartDetail)
                .Reference(cd => cd.Variant)
                .Query()
                .Include(cd => cd.Product)
                .Load();

            return Ok(new { cartDetail = cartDetail, totalPrice = CalculateCart(cartDetail) });
        }

        // HttpPut: api/Action/UpdateVariantQuantity/5/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateVariantQuantity/{id}/{quantityOrdered}")]
        public IActionResult UpdateVariantQuantity(int id, int quantityOrdered)
        {
            Variant variant = _context.Variants.Find(id);

            _context.Entry(variant).State = EntityState.Modified;

            variant.Stock -= quantityOrdered;

            if (variant.Stock < 0)
            {
                return BadRequest("Out of stock");
            }

            try
            {
                _context.SaveChanges();
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

            return Ok(variant);
        }

        // POST: api/Action/CreateCart
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateCart")]
        public async Task<IActionResult> CreateCart(CartDetail cartDetail)
        {
            try
            {
                int variantStock = VariantStock(cartDetail);
                string message = "Insufficient stock. ";

                if(variantStock <= 0)
                {
                    return Ok(new { message = message });
                }

                if (CartDetailExists(cartDetail.MemberID, cartDetail.VariantID))
                {
                    int oldQuantity = CartDetailQuantity(cartDetail);
                    cartDetail.Quantity += oldQuantity;
                    
                    if(cartDetail.Quantity > variantStock)
                    {
                        message += (variantStock - oldQuantity) <= 0 ? "" : "You can only add " + (variantStock - oldQuantity) + " more.";
                        return Ok(new { message = message});
                    }
                    _context.Entry(cartDetail).State = EntityState.Modified;
                }
                else
                {
                    if (cartDetail.Quantity > variantStock)
                    {
                        message += "You can only add " + variantStock + " more.";
                        return Ok(new { message = message });
                    }
                    _context.CartDetails.Add(cartDetail);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok();
        }

        // DELETE: api/Action/DeleteCartDetail/5/5
        [HttpDelete("DeleteCartDetail/{memberID}/{variantID}")]
        public async Task<IActionResult> DeleteCartDetail(int memberID, int variantID)
        {
            var cartDetail = await _context.CartDetails.FindAsync(memberID, variantID);
            if (cartDetail == null)
            {
                return NotFound();
            }



            _context.CartDetails.Remove(cartDetail);

            try
            {
                await _context.SaveChangesAsync();

                _context.Entry(cartDetail).State = EntityState.Unchanged;

            } catch (Exception)
            {
                throw;
            }


            return Ok(new { totalPrice = CalculateCart(cartDetail) });

        }

        // DELETE: api/DeleteCart/5
        [HttpDelete("DeleteCart/{id}")]
        public IActionResult DeleteCart(int id)
        {
            try
            {
                var cartDetail = _context.CartDetails.Where(cd => cd.MemberID == id);
                if (cartDetail == null)
                {
                    return NotFound();
                }

                _context.CartDetails.RemoveRange(cartDetail);
            }
            catch (Exception ex) { }

            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/Action/GetProductImages
        [HttpGet("GetProductImages/{product_id}")]
        public async Task<ActionResult<IEnumerable<ProductImage>>> GetProductImages(int product_id)
            => await _context.ProductImages
            .Include(p => p.Product)
            .Where(v => v.ProductID == product_id).ToListAsync();

        // GET: api/Action/GetProductVariants
        [HttpGet("GetProductVariants/{product_id}")]
        public async Task<ActionResult<IEnumerable<Variant>>> GetProductVariants(int product_id)
            => await _context.Variants
            .Include(p => p.Product)
            .Where(v => v.ProductID == product_id)
            .ToListAsync();

        // GET: api/Action/GetProductCategories/5
        [HttpGet("GetProductCategories/{product_id}")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories(int product_id)
          =>  await _context.ProductCategories
                   .Include(p => p.Category)
                   .Include(p => p.Product)
                   .Where(p => p.ProductID == product_id).ToListAsync();


        // GET: api/Action/GetProductCategories/5/5
        [HttpGet("GetProductCategories/{product_id}/{category_id}")]
        public async Task<ActionResult<ProductCategory>> GetProductCategories(int product_id, int category_id)
            => await _context.ProductCategories
                .Include(p => p.Category)
                .Include(p => p.Product).FirstOrDefaultAsync(m => m.ProductID == product_id && m.CategoryID == category_id);

        // DELETE: api/Action/DeleteProductCategory/5/5
        [HttpDelete("DeleteProductCategory/{product_id}/{category_id}")]
        public async Task<IActionResult> DeleteProductCategory(int product_id, int category_id)
        {
            var product = await _context.ProductCategories.FindAsync(product_id, category_id);

            if (product == null)
            {
                return NotFound();

            }
            _context.ProductCategories.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Action/GetProductImage/5
        [HttpGet("GetProductImage/{image_id}")]
        public async Task<ActionResult<ProductImage>> GetProductImage(int image_id)
            => await _context.ProductImages
                .Include(p => p.Product).FirstOrDefaultAsync(m => m.ID == image_id);

        // GET: api/Action/GetProductVariant/5
        [HttpGet("GetProductVariant/{variant_id}")]
        public async Task<ActionResult<Variant>> GetProductVariant(int variant_id)
            => await _context.Variants
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ID == variant_id);

        /// <summary>
        /// Reusable Operations
        /// </summary>
        private bool CartDetailExists(int id)
        {
            return _context.CartDetails.Any(e => e.MemberID == id);
        }

        private bool CartDetailExists(int id, int variant_id)
        {
            return _context.CartDetails.Any(e => e.MemberID == id && e.VariantID == variant_id);
        }

        private int CartDetailQuantity(CartDetail cartDetail)
        {
            return _context.CartDetails
                        .Where(cd => cd.MemberID == cartDetail.MemberID && cd.VariantID == cartDetail.VariantID)
                        .Select(cd => cd.Quantity)
                        .FirstOrDefault();
        }

        private int VariantStock(CartDetail cartDetail)
        {
            return _context.Variants.Where(v => v.ID == cartDetail.VariantID).Select(v => v.Stock).FirstOrDefault();
        }

        private decimal CalculateCart(CartDetail cartDetail)
        {

            decimal totalPrice = _context.CartDetails.Where(cd => cd.MemberID == cartDetail.MemberID)
                                                .Sum(cd => cd.Quantity * cd.Variant.Product.Price);

            return totalPrice;
        }
    }
}
