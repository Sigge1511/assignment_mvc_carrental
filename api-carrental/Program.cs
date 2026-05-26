using api_carrental.Data;
using api_carrental.Dtos;
using api_carrental.Models;
using api_carrental.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args); // Lade till args här igen för standardhantering

// 1. Hämta och förbered Connection String FÖRST
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Retrieve my AZ password from proj user secrets och byt ut i strängen
var secretPassword = builder.Configuration["DbPassword"];
connectionString = connectionString.Replace("{DbPassword}", secretPassword);

// 2. Registrera DbContext med den FÄRDIGA strängen
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// 3. Register Identity services
builder.Services.AddIdentity<ApplicationUserDto, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGenerationMustBeLongEnough";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CarRentalAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CarRentalMVC";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Remove delay of token expiration
    };
});

builder.Services.AddAuthorization();

//***************** API STUFF *****************
builder.Services.AddScoped<IVehicleRepo, VehicleRepo>();
builder.Services.AddScoped<IBookingRepo, BookingRepo>();
builder.Services.AddScoped<IApplicationUser, ApplicationUserRepo>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// SEEDNING AV ROLLER OCH ADMIN (Ändrat till ApplicationUser)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUserDto>>();

    // Create roles if they don't exist
    string[] roles = { "Admin", "Customer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
            if (!roleResult.Succeeded)
            {
                Console.WriteLine($"Failed to create role '{role}':");
                foreach (var error in roleResult.Errors)
                {
                    Console.WriteLine($"  Error: {error.Code} - {error.Description}");
                }
            }
            else
            {
                Console.WriteLine($"Role '{role}' created successfully.");
            }
        }
    }

    // Create admin account
    var adminEmail = "sigge@site.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new ApplicationUserDto
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdmin, "Sally123!");

        if (result.Succeeded)
        {
            var addToRoleResult = await userManager.AddToRoleAsync(newAdmin, "Admin");
            if (addToRoleResult.Succeeded)
            {
                Console.WriteLine($"Admin user '{adminEmail}' created and added to Admin role successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to add admin role to user '{adminEmail}':");
                foreach (var error in addToRoleResult.Errors)
                {
                    Console.WriteLine($"  Error: {error.Code} - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($"Failed to create admin user '{adminEmail}':");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"  Error: {error.Code} - {error.Description}");
            }
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (addToRoleResult.Succeeded)
            {
                Console.WriteLine($"Admin role added to existing user '{adminEmail}'.");
            }
            else
            {
                Console.WriteLine($"Failed to add admin role to existing user '{adminEmail}':");
                foreach (var error in addToRoleResult.Errors)
                {
                    Console.WriteLine($"  Error: {error.Code} - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine($"Admin user '{adminEmail}' already exists with Admin role.");
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Must be before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();