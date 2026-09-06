using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EMEDS_Project.Data;
using EMEDS_Project.Models;

namespace EMEDS_Project.DAL
{
    public class EmedDbContext(DbContextOptions<EmedDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
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
    }
}