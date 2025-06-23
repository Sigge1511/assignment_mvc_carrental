using assignment_mvc_carrental.Areas.Identity.Pages.Account;
using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using assignment_mvc_carrental.Repos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace assignment_mvc_carrental.Controllers
{
    public class ApplicationUserVMController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _appUserRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ApplicationUserVMController(ApplicationDbContext context, IMapper mapper, IApplicationUser appUserRepo, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _mapper = mapper;
            _appUserRepo = appUserRepo;
            _userManager = userManager;
            _signInManager = signInManager;
        }

//***********************************************************************************************************************

        // GET: CustomerVM/Create

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/CustomerVM/Create.cshtml", new UserInputViewModel());
        }

        // POST: CustomerVM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserInputViewModel newuserVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Error. Please check the input.";
                return View(newuserVM);
            }

            try
            {
                var customerVM = _mapper.Map<CustomerViewModel>(newuserVM);

                // Skapa användaren via repo – och få IdentityResult tillbaka
                var result = await _appUserRepo.AddCustomerAsync(customerVM);

                if (result.Succeeded)
                {
                    //om skapandet lyckades, tilldela rollen "Customer"
                    var user = await _userManager.FindByEmailAsync(customerVM.Email);
                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");
                    }

                    TempData["SuccessMessage"] = "New customer created!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Unexpected error. Please try again.";
                return View(newuserVM);
            }
            catch
            {
                TempData["ErrorMessage"] = "Unexpected error. Please try again.";
                return View(newuserVM);
            }
        }


        //***********************************************************************************************************************

        // GET: CustomerVM/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: CustomerVM/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserInputViewModel userInput)
        {
            if (ModelState.IsValid)
            {
                var customerVM = _mapper.Map<CustomerViewModel>(userInput); //mappa om till en CustomerViewModel
                await _appUserRepo.AddCustomerAsync(customerVM); //skicka till repot

                TempData["SuccessMessage"] = "Reservation successfully created!";

                return RedirectToAction(nameof(RegisterConfirmation));
            }
            return View(userInput);
        }

        public IActionResult RegisterConfirmation()
        {
            return View();
        }

//***********************************************************************************************************************

        // GET: CustomerVM
        public async Task<IActionResult> Index()
        {
            {
                //hämta ALLA
                var allUsers = await _userManager.Users.ToListAsync();

                //skapa lista för alla kunder
                var customerUsers = new List<ApplicationUser>();

                //loopa igenom alla användare och kolla om de har rollen "Customer"
                foreach (var user in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Customer")) 
                    {
                        customerUsers.Add(user); //lägg till i lista
                    }
                }
                // mappa om till CustomerViewModel
                var vmList = _mapper.Map<List<CustomerViewModel>>(customerUsers);

                return View("~/Views/CustomerVM/Index.cshtml", vmList);
            }
        }

//***********************************************************************************************************************

        // GET: CustomerVM/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var customerVM = _mapper.Map<CustomerViewModel>(user);
            return View("~/Views/CustomerVM/Edit.cshtml", customerVM);
        }                           

        // POST: CustomerVM/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel CustomerVM)
        {            
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(CustomerVM.Id);
                if (user == null)
                    return NotFound();

                // Uppdatera fält manuellt
                user.FirstName = CustomerVM.FirstName;
                user.LastName = CustomerVM.LastName;
                user.Email = CustomerVM.Email;
                user.PhoneNumber = CustomerVM.PhoneNumber;
                user.Address = CustomerVM.Address;
                user.City = CustomerVM.City;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Customer information updated!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Failed to update customer information. Please try again.";
                return View("~/Views/CustomerVM/Edit.cshtml", CustomerVM);
            }
            TempData["ErrorMessage"] = "Error. Please try again.";
            return View("~/Views/CustomerVM/Edit.cshtml", CustomerVM);
        }


//***********************************************************************************************************************

        // GET: CustomerVM/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var customerVM = _mapper.Map<CustomerViewModel>(user);
            return View("~/Views/CustomerVM/Delete.cshtml", customerVM);
        }

        // POST: CustomerVM/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Customer has been deleted";
                return RedirectToAction("Index", "ApplicationUserVM");
            }


            // Skicka tillbaka till samma vy om något gick fel
            TempData["ErrorMessage"] = "Something went wrong. Try again.";
            return View(user); // eller View("~/Views/CustomerVM/Delete.cshtml", newuserVM);
        }

//***********************************************************************************************************************

        // GET: CustomerVM/UserPage
        [Authorize]
        public async Task<IActionResult> UserPage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _appUserRepo.GetUserWithBookingsAsync(userId);
            if (user == null) return NotFound();

            var vm = _mapper.Map<CustomerViewModel>(user);

            return View("~/Views/CustomerVM/UserPage.cshtml", vm);
        }


//***********************************************************************************************************************
        [HttpGet("/admin")]
        public IActionResult AdminLogin()
        {
            return View("~/Views/AdminViews/AdminLogin.cshtml");
        }

        [HttpPost("/admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email); //leta användare

            if (user != null && await _userManager.IsInRoleAsync(user, "Admin")) //kolla om användaren är admin
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("AdminPanel");  //gå vidare vid lyckad inloggning
                }
            }
            // Om inloggningen misslyckas, skicka tillbaka till inloggningssidan med felmeddelande
            TempData["ErrorMessage"] = "Invalid login or you are not an admin.";
            return View("~/Views/AdminViews/AdminLogin.cshtml");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminPanel()
        {
            return View("~/Views/AdminViews/AdminPanel.cshtml");
        }


        //Details används inte

        //// GET: CustomerVM/Details/5
        //public async Task<IActionResult> Details(string? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var customerViewModel = await _context.AppUserSet
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (customerViewModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(customerViewModel);
        //}


        //private bool CustomerViewModelExists(string id)
        //{
        //    return _context.AppUserSet.Any(e => e.Id == id);
        //}
    }
}
