using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;

namespace assignment_mvc_carrental.Controllers
{
    public class VehicleVMController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehicleVMController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VehicleViewModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.VehicleViewModel.ToListAsync());
        }

        // GET: VehicleViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleViewModel = await _context.VehicleViewModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleViewModel == null)
            {
                return NotFound();
            }

            return View(vehicleViewModel);
        }

        // GET: VehicleViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,PricePerDay,IsAvailable,Description,ImageUrl1,ImageUrl2")] VehicleViewModel vehicleViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicleViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleViewModel);
        }

        // GET: VehicleViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleViewModel = await _context.VehicleViewModel.FindAsync(id);
            if (vehicleViewModel == null)
            {
                return NotFound();
            }
            return View(vehicleViewModel);
        }

        // POST: VehicleViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,PricePerDay,IsAvailable,Description,ImageUrl1,ImageUrl2")] VehicleViewModel vehicleViewModel)
        {
            if (id != vehicleViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicleViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleViewModelExists(vehicleViewModel.Id))
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
            return View(vehicleViewModel);
        }

        // GET: VehicleViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleViewModel = await _context.VehicleViewModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleViewModel == null)
            {
                return NotFound();
            }

            return View(vehicleViewModel);
        }

        // POST: VehicleViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicleViewModel = await _context.VehicleViewModel.FindAsync(id);
            if (vehicleViewModel != null)
            {
                _context.VehicleViewModel.Remove(vehicleViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleViewModelExists(int id)
        {
            return _context.VehicleViewModel.Any(e => e.Id == id);
        }
    }
}
