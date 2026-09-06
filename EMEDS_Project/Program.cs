using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EMEDS_Project.Data;
using EMEDS_Project.DAL;
using EMEDS_Project.Repository;

namespace EMEDS_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString =
                builder.Configuration.GetConnectionString("EmedDbContext")
                ?? throw new InvalidOperationException(
                    "Connection string 'EmedDbContext' not found.");

            // Register Entity Framework Core DbContext
            builder.Services.AddDbContext<EmedDbContext>(
                options => options.UseSqlServer(connectionString));

            // Configure ASP.NET Core Identity
            builder.Services.AddDefaultIdentity<ApplicationUser>(
                options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<EmedDbContext>();

            builder.Services.AddScoped<IMedicineRepo, MedicineRepo>();
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();

            // Add MVC services
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Initialize roles
            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider
                        .GetRequiredService<RoleManager<IdentityRole>>();

                var userManager =
    scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();

                await DbInitializer.InitializeAsync(
                    roleManager,
                    userManager);
            }

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages();

            app.Run();
        }
    }
}