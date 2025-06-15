using assignment_mvc_carrental.Classes;
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

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>(); //säg vilken class/modell som ska användas för Identity

            builder.Services.AddControllersWithViews();

            builder.Services.AddMemoryCache(); //Lägg till minnescache för att lagra data i minnet

            builder.Services.AddSession(options => //Lägg till session för att lagra data mellan requests
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60); //sessionen timeoutar efter 60 minuter

                //options.Cookie.HttpOnly = true; //sessionen är endast tillgänglig via HTTP
                //options.Cookie.IsEssential = true; //sessionen är nödvändig för applikationen
            });

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
