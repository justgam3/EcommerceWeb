using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceWebApi.Models
{
    public class ShopDBContext : DbContext
    {
        public ShopDBContext(DbContextOptions<ShopDBContext> options) : base(options)
        {

        }

        public DbSet<CartDetail> CartDetails { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<MemberWishlist> MemberWishlists { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails{ get; set; }

        public DbSet<Product> Products{ get; set; }

        public DbSet<ProductCategory> ProductCategories{ get; set; }

        public DbSet<ProductImage> ProductImages{ get; set; }

        public DbSet<Variant> Variants{ get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartDetail>().ToTable("CartDetail");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Member>().ToTable("Member");
            modelBuilder.Entity<CartDetail>().ToTable("CartDetail");
            modelBuilder.Entity<MemberWishlist>().ToTable("MemberWishlist");
            modelBuilder.Entity<Message>().ToTable("Message");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");
            modelBuilder.Entity<ProductImage>().ToTable("ProductImage");
            modelBuilder.Entity<Variant>().ToTable("Variant");

            modelBuilder.Entity<CartDetail>()
                .HasKey(cd => new { cd.MemberID, cd.VariantID});
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderID, od.VariantID });
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductID, pc.CategoryID });
            modelBuilder.Entity<MemberWishlist>()
                .HasKey(mw => new { mw.MemberID, mw.ProductID });
            modelBuilder.Entity<Message>()
                .HasKey(m => new { m.MemberID, m.ProductID});

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Variant>()
                .HasMany(v => v.OrderDetails)
                .WithOne(v => v.Variant)
                .HasForeignKey(v => v.VariantID)
                .OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}
