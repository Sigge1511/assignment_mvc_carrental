using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using AutoMapper;
using assignment_mvc_carrental.Repos;

namespace assignment_mvc_carrental.Controllers
{
    public class VehicleVMController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IVehicle _vehicleRepo; // Added dependency for IVehicle

        public VehicleVMController(ApplicationDbContext context, IMapper mapper, IVehicle vehicleRepo)
        {
            _context = context;
            _mapper = mapper;
            _vehicleRepo = vehicleRepo; // Initialize the IVehicle repository
        }


        //***********************************************************************************************************************
        // GET: VehicleViewModels
        [Route("allvehicles")]
        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleRepo.GetAllVehiclesAsync(); //hämtar alla fordon från databasen genom interface -> repo -> db
            var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles); //mappar fordonen till en lista av VehicleViewModel
            return View("~/Views/VehicleViewModels/Index.cshtml", vehicleVMList); //returnerar VMlistan och skickar till rätt vy i trädet
        }


        //***********************************************************************************************************************
        // GET: VehicleViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleViewModel = await _context.VehicleSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleViewModel == null)
            {
                return NotFound();
            }

            return View("~/Views/VehicleViewModels/Details.cshtml", vehicleViewModel); //returnerar vy i trädet + detaljerna för fordonet
        }


        //***********************************************************************************************************************
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


        //***********************************************************************************************************************
        // GET: VehicleViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleViewModel = await _context.VehicleSet.FindAsync(id);
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


        //***********************************************************************************************************************
        // GET: VehicleViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleViewModel = await _context.VehicleSet
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
            var vehicleViewModel = await _context.VehicleSet.FindAsync(id);
            if (vehicleViewModel != null)
            {
                _context.VehicleSet.Remove(vehicleViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //***********************************************************************************************************************
        private bool VehicleViewModelExists(int id)
        {
            return _context.VehicleSet.Any(e => e.Id == id);
        }
    }
}
