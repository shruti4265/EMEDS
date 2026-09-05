using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EMEDS_Project.Data;

namespace EMEDS_Project.DAL
{
    public class EmedDbContext(DbContextOptions<EmedDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
    }
}