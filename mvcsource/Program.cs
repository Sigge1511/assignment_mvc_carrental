using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using AutoMapper;

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

            //hjälper till att skicka till inlogg om man vill hyra bil utan konto tex
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();



            //**************    MAPPER  **********************************************************************

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());



            //************ L�GG TILL ALLA REPOS H�R   *****************************************************************
            


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

            app.UseAuthentication(); // kopplat till inlogg osv

            app.UseAuthorization();

            // Area routes for Identity
            app.MapAreaControllerRoute(
                name: "Identity",
                areaName: "Identity",
                pattern: "{controller}/{action=Index}/{id?}");

            // Default controller route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Razor Pages (including Identity pages)
            app.MapRazorPages();

            app.Run();
        }
    }
}
