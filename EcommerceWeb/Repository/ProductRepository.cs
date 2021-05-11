using EcommerceWebApi.Filter;
using EcommerceWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EcommerceWebApi.Repository
{
    public class ProductRepository
    {
        private ShopDBContext _context;

        public ProductRepository(ShopDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        //public async Task<List<Product>> GetAllProductByPagination(PaginationFilter validFilter)
        //{
        //   return await _context.Products
        //       .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        //       .Take(validFilter.PageSize)
        //       .ToListAsync();
        //}

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public void InsertProduct(Product product)
        {
            _context.Products.Add(product);
        }
        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
        }
        public void UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
