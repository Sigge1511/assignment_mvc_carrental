using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using assignment_mvc_carrental.Repos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace assignment_mvc_carrental.Controllers
{
    public class ApplicationUserCMController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IApplicationUser _appUserRepo;
        private readonly IVehicle _vehicleRepo;

        public ApplicationUserCMController(ApplicationDbContext context, IMapper mapper, IApplicationUser appUserRepo, IVehicle vehicleRepo)
        {
            _context = context;
            _mapper = mapper;
            _appUserRepo = appUserRepo;
            _vehicleRepo = vehicleRepo;
        }


        //***********************************************************************************************************************

        // GET: CustomerVM/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CustomerVM/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,Address,City")] CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customerViewModel);
        }


        //***********************************************************************************************************************

        // GET: CustomerVM
        public async Task<IActionResult> Index()
        {
            return View(await _context.AppUserSet.ToListAsync());
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
