using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceWebApi.Models;
using EcommerceWebApi.DTO;
using EcommerceWebApi.Filter;

namespace EcommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopDBContext _context;

        public ProductsController(ShopDBContext context)
        {
            _context = context;
        }


        // GET: api/Products/GetProductCount
        [HttpGet("GetProductCount")]
        public int GetProductCount()
        {
            return _context.Products.Count();
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/GetProductsByPagination?PageNumber=1&PageSize=6
        [HttpGet("GetProductsByPagination")]
        public async Task<ActionResult<PaginationFilter<Product>>> GetProductsByPagination([FromQuery] PaginationFilter<Product> filter, string orderBy)
        {
            var products = from p in _context.Products
                           select p;

            orderBy = String.IsNullOrEmpty(orderBy) ? "created_at" : orderBy;
            switch (orderBy)
            {
                case "created_at":
                    products = products.OrderByDescending(p => p.CreatedAt);
                    break;
                case "updated_at":
                    products = products.OrderByDescending(p => p.UpdatedAt);
                    break;
            }
            var pagedData = await products.Skip((filter.PageNumber - 1) * filter.PageSize)
                                        .Take(filter.PageSize)    
                                        .ToListAsync();

            var totalPages = (int)Math.Ceiling(products.Count() / (double)filter.PageSize);

            return new PaginationFilter<Product>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                PagedData = pagedData,
                TotalPages = totalPages
            };
        }

        // GET: api/Products/GetProductImagesAndCategoriesByPagination?PageNumber=1&PageSize=6
        [HttpGet("GetProductImagesAndCategoriesByPagination")]
        public async Task<ActionResult<PaginationFilter<Product>>> GetProductImagesAndCategoriesByPagination([FromQuery] PaginationFilter<Product> filter)
        {
            var products = _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategories)
                .ThenInclude(p => p.Category)
                .Where(p => p.IsActive == true && p.Variants.Count > 0);

            var pagedData = await products.Skip((filter.PageNumber - 1) * filter.PageSize)
                                        .Take(filter.PageSize)
                                        .ToListAsync();

            var totalPages = (int)Math.Ceiling(products.Count() / (double)filter.PageSize);

            return new PaginationFilter<Product>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                PagedData = pagedData,
                TotalPages = totalPages
            };
        }

        //await _context.Products
        //    .Include(p => p.ProductImages)
        //        .Include(p => p.ProductCategories)
        //        .ThenInclude(p => p.Category)
        //        .Where(p => p.IsActive == true && p.Variants.Count > 0)
        //        .ToListAsync();


        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // GET: api/Products/5/GetProductDetails
        [HttpGet("{id}/GetProductDetails")]
        public async Task<ActionResult<Product>> GetProductDetails(int id)
        {
            var product = await _context.Products.SingleAsync(p => p.ID == id);

            await _context.Entry(product)
                .Collection(p => p.Variants)
                .LoadAsync();

            await _context.Entry(product)
                .Collection(p => p.ProductImages)
                .LoadAsync();

            await _context.Entry(product)
                .Collection(p => p.MemberWishlists)
                .Query()
                .Include(p => p.Member)
                .LoadAsync();

            if (product == null)
            {
                return NotFound();
            }

            return new Product
            {
                ID = product.ID,
                CreatedAt = product.CreatedAt,
                Description = product.Description,
                MemberWishlists = product.MemberWishlists,
                Messages = product.Messages,
                Price = product.Price,
                ProductImages = product.ProductImages,
                ProductName = product.ProductName,
                Variants = product.Variants
            };
        }

        // GET: api/Products/5/GetAdminProductDetails
        [HttpGet("{id}/GetAdminProductDetails")]
        public async Task<ActionResult<Product>> GetAdminProductDetails(int id)
        {
            var product = await _context.Products.SingleAsync(p => p.ID == id);

            await _context.Entry(product)
                .Collection(p => p.Variants)
                .LoadAsync();

            await _context.Entry(product)
                .Collection(p => p.ProductImages)
                .LoadAsync();

            await _context.Entry(product)
                .Collection(p => p.ProductCategories)
                .Query()
                .Include(p => p.Category)
                .LoadAsync();

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // PUT: api/Products/5/PutAdminProduct
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/PutAdminProduct")]
        public async Task<IActionResult> PutAdminProduct(int id, Product product)
        {
            if (id != product.ID)
            {
                return BadRequest();
            }

            Product parent = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.ProductImages)
                .Include(p => p.Variants)
                .ThenInclude(p => p.OrderDetails)
                .FirstAsync(p => p.ID == product.ID);

            //if (parent != null)
            //{
            //    _context.Entry(parent).CurrentValues.SetValues(product);

            //    foreach (var child in parent.Variants)
            //    {
            //        if (!product.Variants.Any(c => c.ID == child.ID))
            //            _context.Variants.Remove(child);
            //    }

            //    foreach (var childModel in product.Variants)
            //    {
            //        var existingChild = parent.Variants
            //            .Where(c => c.ID == childModel.ID)
            //            .SingleOrDefault();

            //        if (existingChild != null)
            //            // Update child
            //            _context.Entry(existingChild).CurrentValues.SetValues(childModel);
            //        else
            //        {
            //            // Insert child
            //            var newChild = new Variant
            //            {
            //                Stock = childModel.Stock,
            //                Type = childModel.Type
            //            };
            //            parent.Variants.Add(newChild);
            //        }

            //    }
            //}

            if (parent != null)
            {
                _context.Entry(parent).CurrentValues.SetValues(product);
                parent.ProductImages = product.ProductImages;
                parent.ProductCategories = product.ProductCategories;
                parent.Variants = product.Variants;
            }
            //parent.ID = product.ID;
            //parent.ProductName = product.ProductName;
            //parent.Price = product.Price;
            //parent.IsActive = product.IsActive;
            //parent.CreatedAt = product.CreatedAt;
            //parent.Description = product.Description;
            //parent.ProductImages = product.ProductImages;
            //parent.ProductCategories = product.ProductCategories;
            //parent.Variants = product.Variants;


            _context.Entry(parent).State = EntityState.Modified;

            try
            {

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {

            }


            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ID }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }

       
    }
}
