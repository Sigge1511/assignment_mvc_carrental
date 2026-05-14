using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using assignment_mvc_carrental.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace assignment_mvc_carrental.Controllers
{
    public class ApplicationUserVMController : Controller
    {
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;

        //Kan vara bra för ev jsonstrul
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ApplicationUserVMController(IMapper mapper, HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _mapper = mapper;
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
        }

        //***********************************************************************************************************************

        //GET: CustomerVM/Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/CustomerVM/Create.cshtml", new UserInputViewModel());
        }

       //POST: CustomerVM/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserInputViewModel newuserVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error. Please Try again.";
                return View(newuserVM);
            }

            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                // Mappa Input-VM till DTO för API:et. CustomerViewModel funkar som DTO
                var newCustomerForApi = _mapper.Map<CustomerViewModel>(newuserVM);

                // 3. Ropa på API:et
                // OBS: "AppUserDt" måste matchas mot din endpoint i API:et.
                var response = await client.PostAsJsonAsync("AppUserDt", newCustomerForApi);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "New customer created!";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Felhantering för misslyckat API-anrop
                    TempData["ErrorMessage"] = "API failed to create user. Check API logs for details.";
                    return View(newuserVM);
                }
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Connection error: Could not reach API. Please ensure the API is running.";
                return View(newuserVM);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return View(newuserVM);
            }
        }


        //***********************************************************************************************************************

         //GET: CustomerVM/Register
        public IActionResult Register()
        {
            return View();
        }

        //POST: CustomerVM/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserInputViewModel userInput)
        {
            if (!ModelState.IsValid)
            {
                return View(userInput);
            }
            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                // 1. Mappa Input-VM till DTO:t
                var newCustomerForApi = _mapper.Map<CustomerViewModel>(userInput);

                // 2. Ropa på API:et (Samma endpoint som Create)
                var response = await client.PostAsJsonAsync("AppUserDt", newCustomerForApi);

                if (response.IsSuccessStatusCode)
                {
                    // OBS: Jag har ändrat meddelandet för att vara mer logiskt för registrering.
                    TempData["SuccessMessage"] = "Registration successful! You can now log in.";

                    return RedirectToAction(nameof(RegisterConfirmation));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    // Konflikt (t.ex. E-postadressen används redan)
                    TempData["ErrorMessage"] = "Registration failed. Do you already have an account?";
                    return View(userInput);
                }
                else
                {
                    TempData["ErrorMessage"] = "Registration failed. Please check the details and try again.";
                    return View(userInput);
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return View(userInput);
            }
        }

        public IActionResult RegisterConfirmation()
        {
            return View();
        }

        //***********************************************************************************************************************

         //GET: CustomerVM
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                // 1. Hämta data, mvc-Model matchar Dto hos api.
                var apiUserList = await client.GetFromJsonAsync<List<ApplicationUser>>("AppUserDt");

                if (apiUserList == null)
                {
                    return View("~/Views/CustomerVM/Index.cshtml", new List<CustomerViewModel>());
                }
                // 2. Mappa om
                var vmList = _mapper.Map<List<CustomerViewModel>>(apiUserList);
                return View("~/Views/CustomerVM/Index.cshtml", vmList);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Connection error: Could not reach API. Details: {ex.Message}";
                return View("~/Views/CustomerVM/Index.cshtml", new List<CustomerViewModel>());
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return View("~/Views/CustomerVM/Index.cshtml", new List<CustomerViewModel>());
            }
        }

//***********************************************************************************************************************

         //GET: CustomerVM/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            // 1. Kontrollera ID
            if (string.IsNullOrEmpty(id))
                return NotFound();
            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var apiUrl = $"AppUserDt/{id}";
                var user = await client.GetFromJsonAsync<ApplicationUser>(apiUrl);
                if (user == null)
                {
                    return NotFound();
                }

                // 3. Mappa till ViewModel
                var customerVM = _mapper.Map<CustomerViewModel>(user);
                return View("~/Views/CustomerVM/Edit.cshtml", customerVM);
            }
            catch (HttpRequestException)
            {
                // Fånga fel om klienten inte kunde ansluta till API:et alls
                TempData["ErrorMessage"] = "Connection error: Could not connect to the user API.";
                return NotFound();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return NotFound();
            }
        }

        //POST: CustomerVM/Edit/5
        //To protect from overposting attacks, enable the specific properties you want to bind to.
        //For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel CustomerVM)
        {
            // 1. Lokalt fel, t.ex. att formuläret inte validerade
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error. Please try again.";
                return View("~/Views/CustomerVM/Edit.cshtml", CustomerVM);
            }

            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                // 2. Kontrollera om ID finns
                if (string.IsNullOrEmpty(CustomerVM.Id))
                {
                    TempData["ErrorMessage"] = "Cannot update user: ID is missing.";
                    return View("~/Views/CustomerVM/Edit.cshtml", CustomerVM);
                }

                // 3. Skapa apiUrl för att ropa och skicka med id
                var url = $"AppUserDt/{CustomerVM.Id}";

                // Skicka CustomerVM till min nya apiUrl
                var response = await client.PutAsJsonAsync(url, CustomerVM);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Customer information updated!";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Felhantering för misslyckat anrop
                    TempData["ErrorMessage"] = "Failed to update customer information via API. Please check API logs.";
                    return View("~/Views/CustomerVM/Edit.cshtml", CustomerVM);
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred in the MVC client.";
                return View("~/Views/CustomerVM/Edit.cshtml", CustomerVM);
            }
        }


        //***********************************************************************************************************************

         //GET: CustomerVM/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var apiUrl = $"AppUserDt/{id}";

                // Hämta data som din ApplicationUser-modell (som matchar API:ets DTO)
                var user = await client.GetFromJsonAsync<ApplicationUser>(apiUrl);

                if (user == null)
                {
                    // API:et kunde inte hitta användaren
                    return NotFound();
                }

                // 3. Mappa den hämtade ApplicationUser-modellen till ViewModellen
                var customerVM = _mapper.Map<CustomerViewModel>(user);

                // 4. Returnera vyn (Delete.cshtml, som specificerats)
                return View("~/Views/CustomerVM/Delete.cshtml", customerVM);
            }
            catch (HttpRequestException)
            {
                // Fånga fel om klienten inte kunde ansluta till API:et alls
                TempData["ErrorMessage"] = "Connection error: Could not connect to the user API.";
                return NotFound();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        //POST: CustomerVM/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Cannot delete user: ID is missing.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var apiUrl = $"AppUserDt/{id}";

                // Skickar en HTTP DELETE-förfrågan till API:et
                var response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Customer has been deleted";
                    return RedirectToAction(nameof(Index));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Error: Customer not found in API.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Felhantering för misslyckat API-anrop
                    TempData["ErrorMessage"] = "API failed to delete customer. Try again.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred in the MVC client.";
                return RedirectToAction(nameof(Index));
            }
        }

//***********************************************************************************************************************

         //GET: CustomerVM/UserPage
        [Authorize]
        public async Task<IActionResult> UserPage()
        {
            // 1. Hämta användar-ID som tidigare
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var apiUrl = $"AppUserDt/bookings/{userId}";

                // HttpClient anropar nu den nya metoden GetUserWithBookings i API:et
                var user = await client.GetFromJsonAsync<ApplicationUser>(apiUrl);

                if (user == null)
                {
                    return NotFound();
                }

                // 3. Mappa den hämtade modellen till ViewModellen
                var vm = _mapper.Map<CustomerViewModel>(user);

                return View("~/Views/CustomerVM/UserPage.cshtml", vm);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Connection error: Could not connect to the user API.";
                return NotFound();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return NotFound();
            }
        }


//***********************************************************************************************************************
        [HttpGet("/admin")]
        public IActionResult AdminLogin()
        {
            return View("~/Views/AdminViews/AdminLogin.cshtml");
        }

        //[HttpPost("/admin")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AdminLogin(string email, string password)
        //{
        //    var user = await _userManager.FindByEmailAsync(email); //leta användare

        //    if (user != null && await _userManager.IsInRoleAsync(user, "Admin")) //kolla om användaren är admin
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("AdminPanel");  //gå vidare vid lyckad inloggning
        //        }
        //    }
        //    //Om inloggningen misslyckas, skicka tillbaka till inloggningssidan med felmeddelande
        //    TempData["ErrorMessage"] = "Invalid login or you are not an admin.";
        //    return View("~/Views/AdminViews/AdminLogin.cshtml");
        //}

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminPanel()
        {
            return View("~/Views/AdminViews/AdminPanel.cshtml");
        }


        //Details används inte

        // GET: CustomerVM/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var apiUrl = $"AppUserDt/{id}";
                var user = await client.GetFromJsonAsync<ApplicationUser>(apiUrl);
                if (user == null)
                {
                    // API:et kunde inte hitta användaren
                    return NotFound();
                }
                // mappa om
                var customerViewModel = _mapper.Map<CustomerViewModel>(user);
                return View(customerViewModel);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Connection error: Could not connect to the user API.";
                return NotFound();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return NotFound();
            }
        }

        //Ska ej behövas nu när api kan svara med olika felkoder
        //private bool CustomerViewModelExists(string id)
        //{
        //    return _context.AppUserSet.Any(e => e.Id == id);
        //}
    }
}
