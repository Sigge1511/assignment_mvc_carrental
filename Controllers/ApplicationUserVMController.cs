using assignment_mvc_carrental.Areas.Identity.Pages.Account;
using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using assignment_mvc_carrental.Repos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace assignment_mvc_carrental.Controllers
{
    public class ApplicationUserVMController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _appUserRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVehicle _vehicleRepo;

        public ApplicationUserVMController(ApplicationDbContext context, IMapper mapper, IApplicationUser appUserRepo, UserManager<ApplicationUser> userManager, IVehicle vehicleRepo)
        {
            _context = context;
            _mapper = mapper;
            _appUserRepo = appUserRepo;
            _userManager = userManager;
            _vehicleRepo = vehicleRepo;
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

        // GET: CustomerVM/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerViewModel = await _context.AppUserSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerViewModel == null)
            {
                return NotFound();
            }

            return View(customerViewModel);
        }

        

        // GET: CustomerVM/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerViewModel = await _context.AppUserSet.FindAsync(id);
            if (customerViewModel == null)
            {
                return NotFound();
            }
            return View(customerViewModel);
        }

        // POST: CustomerVM/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Address,City")] CustomerViewModel customerViewModel)
        {
            if (id != customerViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerViewModelExists(customerViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customerViewModel);
        }

        // GET: CustomerVM/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerViewModel = await _context.AppUserSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerViewModel == null)
            {
                return NotFound();
            }

            return View(customerViewModel);
        }

        // POST: CustomerVM/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customerViewModel = await _context.AppUserSet.FindAsync(id);
            if (customerViewModel != null)
            {
                _context.AppUserSet.Remove(customerViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerViewModelExists(string id)
        {
            return _context.AppUserSet.Any(e => e.Id == id);
        }
    }
}
