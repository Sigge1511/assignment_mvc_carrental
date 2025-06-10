using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace assignment_mvc_carrental
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services 
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            //**************    MAPPER  **********************************************************************

            builder.Services.AddAutoMapper(typeof(MappingProfile)); //mappning mellan klasser och VMs



            //************ LÄGG TILL ALLA REPOS HÄR   *****************************************************************
            builder.Services.AddScoped<IVehicle, VehicleRepo>();
            builder.Services.AddScoped<IBooking, BookingRepo>();



            //*************************************************************************************************
            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
