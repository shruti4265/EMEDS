using EMEDS_Project.Data;
using EMEDS_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EMEDS_Project.DAL
{
    public class EmedDbContext(DbContextOptions<EmedDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Prescription> Prescriptions { get; set; }
    }
}