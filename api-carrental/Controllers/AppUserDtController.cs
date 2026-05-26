using api_carrental.Data;
using api_carrental.Dtos;
using api_carrental.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace api_carrental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserDtController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IApplicationUser _applicationUser;
        private readonly UserManager<ApplicationUserDto> _userManager;
        private readonly SignInManager<ApplicationUserDto> _signInManager;
        private readonly IConfiguration _configuration;

        public AppUserDtController(ApplicationDbContext applicationDbContext,
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

        //HÄMTA ALLA KUNDER FÖR LISTA OCH ÖVERSIKT
        // GET: api/<AppUserDtController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUserDto>>> GetCustomersIndexAsync()
        {
            try
            {
                var allUsers = await _userManager.Users.ToListAsync();
                var customerUsers = new List<ApplicationUserDto>();

                //loopa igenom alla användare och kolla om de har rollen "Customer"
                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Customer"))
                    {
                        customerUsers.Add(user); //lägg till i lista
                    }
                }
                return Ok(customerUsers);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong. Please try again" + ex.Message);
            }
        }

        //HÄMTA ENSKILD KUND
        // GET api/<AppUserDtController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUserDto>> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong: {ex.Message}");
            }
        }


        // Denna rutt är unik och säger: "Hämta en AppUser som inkluderar bokningar"
        [HttpGet("bookings/{id}")]
        public async Task<ActionResult<ApplicationUserDto>> GetUserWithBookings(string id)
        {
            var user = await _applicationUser.GetUserWithBookingsAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        // LÄGG TILL NY KUND
        // POST api/<AppUserDtController>
        [HttpPost]
        //ALLA SKA KUNNA SKAPA KONTO SJÄLVA
        public async Task<IActionResult> PostUserAsync([FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
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
                    var token = GenerateJwtToken(newUser);

                    return Ok(new
                    {
                        message = "User registered successfully!",
                        userId = newUser.Id,
                        email = newUser.Email,
                        token = token,
                        roles = await _userManager.GetRolesAsync(newUser)
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

        // UPPDATERA KUND
        // PUT api/<AppUserDtController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerAsync(string id, [FromBody] ApplicationUserDto appUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Check if the user is updating their own profile or is an admin
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                if (currentUserId != id && !isAdmin)
                {
                    return Forbid("You can only update your own profile.");
                }

                // Update user fields
                user.FirstName = appUser.FirstName;
                user.LastName = appUser.LastName;
                user.Email = appUser.Email;
                user.UserName = appUser.Email;
                user.PhoneNumber = appUser.PhoneNumber;
                user.Address = appUser.Address;
                user.City = appUser.City;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Customer information updated successfully." });
                }

                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Update failed", errors = errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        // RADERA KUND
        // DELETE api/<AppUserDtController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return BadRequest("Unexpected error. Please try again.");
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok("Customer has been deleted");
                }
                else { return BadRequest("Something went wrong. Try again."); }
            }
            catch
            {
                return BadRequest("Unexpected error. Please try again.");
            }
        }

        //***************************************************************************************************************

        private string GenerateJwtToken(ApplicationUserDto user)
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
