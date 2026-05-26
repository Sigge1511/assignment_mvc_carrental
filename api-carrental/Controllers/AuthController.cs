using api_carrental.Data;
using api_carrental.Dtos;
using api_carrental.Models;
using api_carrental.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_carrental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IApplicationUser _applicationUser;
        private readonly UserManager<ApplicationUser> _userManager; // Ändrad till ApplicationUser
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext applicationDbContext,
                                    IApplicationUser applicationUser,
                                    UserManager<ApplicationUser> userManager, // Ändrad till ApplicationUser
                                    IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _applicationUser = applicationUser;
            _userManager = userManager;
            _configuration = configuration;
        }

        //***************************************************************************************************************

        [HttpPost("admin")] // Ändrad rutt till api/auth/admin (tog bort /)
        public async Task<IActionResult> AdminLogin(LoginUserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);
                if (user == null)
                {
                    return BadRequest("Something went wrong, please try again.");
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, userDto.Password);
                if (!passwordValid)
                {
                    return BadRequest("Something went wrong, please try again.");
                }

                // Check if user is admin
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    return Unauthorized("User is not an admin");
                }

                // Generate JWT Token
                var token = GenerateJwtToken(user, roles);

                return Ok(new
                {
                    message = "Logged in as admin",
                    token = token,
                    user = new
                    {
                        email = user.Email,
                        roles = roles
                    }
                });
            }
            catch (Exception)
            {
                return Problem($"Something went wrong in the {nameof(AdminLogin)}", statusCode: 500);
            }
        }

        //***************************************************************************************************************

        [HttpPost("register")] // Ändrad rutt till api/auth/register
        public async Task<IActionResult> Register(UserRegistrationDto registrationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(registrationDto.Email);
                if (existingUser != null)
                {
                    return Conflict("A user with this email already exists.");
                }

                // Create new user (Ändrad till din riktiga domänmodell ApplicationUser)
                var newUser = new ApplicationUser
                {
                    UserName = registrationDto.Email,
                    Email = registrationDto.Email,
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    PhoneNumber = registrationDto.PhoneNumber,
                    Address = registrationDto.Address,
                    City = registrationDto.City
                };

                // Create user with password
                var result = await _userManager.CreateAsync(newUser, registrationDto.Password);

                if (result.Succeeded)
                {
                    // Assign Customer role
                    await _userManager.AddToRoleAsync(newUser, "Customer");

                    // Generate JWT token
                    var roles = await _userManager.GetRolesAsync(newUser);
                    var token = GenerateJwtToken(newUser, roles);

                    return Ok(new
                    {
                        message = "User registered successfully!",
                        userId = newUser.Id,
                        email = newUser.Email,
                        token = token,
                        roles = roles
                    });
                }

                // Return validation errors
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "User registration failed", errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", details = ex.Message });
            }
        }

        //***************************************************************************************************************

        [HttpPost("login")] // Ändrad rutt till api/auth/login
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);
                if (user == null)
                {
                    return BadRequest("Invalid email or password");
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, userDto.Password);
                if (!passwordValid)
                {
                    return BadRequest("Invalid email or password");
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Generate JWT Token
                var token = GenerateJwtToken(user, roles);

                return Ok(new
                {
                    message = "Login successful",
                    token = token,
                    user = new
                    {
                        email = user.Email,
                        roles = roles
                    }
                });
            }
            catch (Exception)
            {
                return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
            }
        }

        //***************************************************************************************************************

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles) // Ändrad till ApplicationUser
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGenerationMustBeLongEnough";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "CarRentalAPI";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "CarRentalMVC";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add roles as claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24), // Token expires in 24 hours
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}