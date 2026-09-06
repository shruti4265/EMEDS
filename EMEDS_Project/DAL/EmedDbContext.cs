using EMEDS_Project.Data;
using EMEDS_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======
using EMEDS_Project.Data;
using EMEDS_Project.Models;
>>>>>>> origin/feature/chhaya-auth-medicine-category

namespace EMEDS_Project.DAL
{
    public class EmedDbContext : IdentityDbContext<ApplicationUser>
    {
<<<<<<< HEAD
        public EmedDbContext(DbContextOptions<EmedDbContext> options)
            : base(options)
        {
        }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Inventory>()
                .HasOne(i => i.Supplier)
                .WithMany()
                .HasForeignKey(i => i.SupplierId)
                .IsRequired(false);
            builder.Entity<Order>()
    .HasOne(o => o.Payment)
    .WithOne(p => p.Order)
    .HasForeignKey<Payment>(p => p.OrderId);
        }

=======
        public DbSet<Medicine> Medicines { get; set; }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Medicine>()
                .HasOne(m => m.Category)
                .WithMany()
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
>>>>>>> origin/feature/chhaya-auth-medicine-category
    }
}