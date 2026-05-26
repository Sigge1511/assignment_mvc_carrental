using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies; // Tillagd för cookie-hantering utan Identity

namespace api_carrental
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddHttpClient("CarRentalAPI", client =>
            {
                // API-projektet körs på HTTPS port 7045 (enligt launchSettings.json)
                client.BaseAddress = new Uri("https://localhost:7045/api/");
            });

            // REGISTRERING AV COOKIE-AUTENTISERING (Ersätter Identity i MVC-appen)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    // Hit skickas användaren om de försöker nå en [Authorize]-sida utan att vara inloggade
                    options.LoginPath = "/ApplicationUserVM/Login";
                    options.LogoutPath = "/ApplicationUserVM/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Hur länge de förblir inloggade
                });

            builder.Services.AddControllersWithViews();

            //************** MAPPER  **********************************************************************

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            //************ LÄGG TILL ALLA REPOS HÄR   *****************************************************************



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

            // Viktigt: Autentisering först, sedan auktorisering
            app.UseAuthentication();
            app.UseAuthorization();

            // Standard MVC-rutt som pekar på din startsida
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}