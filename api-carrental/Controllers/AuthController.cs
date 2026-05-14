using api_carrental.Data;
using api_carrental.Dtos;
using api_carrental.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly UserManager<ApplicationUserDto> _userManager;
        private readonly SignInManager<ApplicationUserDto> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext applicationDbContext,
                                    IApplicationUser applicationUser,
                                    UserManager<ApplicationUserDto> userManager,
                                    SignInManager<ApplicationUserDto> signInManager,
                                    IConfiguration configuration)
        {
            _applicationDbContext = applicationDbContext;
            _applicationUser = applicationUser;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        //***************************************************************************************************************

        [HttpPost("/admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(LoginUserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);
                var passwordValid = await _userManager.CheckPasswordAsync(user, userDto.Password);
                // Bool som jämför password från user och det som skickas in userDto.Password dvs

                if (user == null || passwordValid == false) // För att inte ge ut om bara password eller User är felaktigt.
                {
                    return BadRequest("Something went wrong, please try again."); // Skickas
                }

                // Check if user is admin
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    return Unauthorized("User is not an admin");
                }

                // Generate JWT Token
                var token = GenerateJwtToken(user, roles);

                return Ok(new {
                    message = "Logged in as admin",
                    token = token,
                    user = new {
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

        [HttpPost("/register")]
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

                // Create new user
                var newUser = new ApplicationUserDto
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

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);
                var passwordValid = await _userManager.CheckPasswordAsync(user, userDto.Password);

                if (user == null || passwordValid == false)
                {
                    return BadRequest("Invalid email or password");
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Generate JWT Token
                var token = GenerateJwtToken(user, roles);

                return Ok(new {
                    message = "Login successful",
                    token = token,
                    user = new {
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

        private string GenerateJwtToken(ApplicationUserDto user, IList<string> roles)
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
