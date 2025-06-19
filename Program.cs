using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace assignment_mvc_carrental
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services 
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddDatabaseDeveloperPageExceptionFilter();



            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
                (Microsoft.AspNetCore.Identity.IdentityOptions options) =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders(); //detta ska hjälpa Identity fungera




            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();



            //**************    MAPPER  **********************************************************************

            builder.Services.AddAutoMapper(typeof(MappingProfile)); //mappning mellan klasser och VMs



            //************ L�GG TILL ALLA REPOS H�R   *****************************************************************
            builder.Services.AddScoped<IVehicle, VehicleRepo>();
            builder.Services.AddScoped<IBooking, BookingRepo>();
            builder.Services.AddScoped<IApplicationUser, ApplicationUserRepo>();


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

            using (var scope = app.Services.CreateScope()) //skapa en admin och usermanager 
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roles = { "Admin", "Customer" }; //fyller identitys userrole-tabell

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                    //skapar nya om de inte finns i db redan
                }

                // Skapa adminkonto 
                var adminEmail = "sigge@site.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail); //leta efter admin i db
                if (adminUser == null)
                {
                    var newAdmin = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
                    //skapa en ny admin om den saknas
                    var result = await userManager.CreateAsync(newAdmin, "Sally123!");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                    }
                    else
                    {
                        // Logga ut fel
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Fel: {error.Code} - {error.Description}");
                        }
                    }
                }
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // kopplat till inlogg osv

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
