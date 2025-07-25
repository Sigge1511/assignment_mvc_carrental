﻿using assignment_mvc_carrental.Classes;
using assignment_mvc_carrental.Data;
using assignment_mvc_carrental.Models;
using assignment_mvc_carrental.Repos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace assignment_mvc_carrental.Controllers
{
    public class VehicleVMController : Controller
    {
        private readonly ApplicationDbContext _context; // dependency för Context
        private readonly IMapper _mapper; // dependency för IMapper
        private readonly IVehicle _vehicleRepo; // dependency för IVehicle
        private readonly Microsoft.AspNetCore.Identity.UserManager<Classes.ApplicationUser> _userManager;

        public VehicleVMController(ApplicationDbContext context, 
                                    IMapper mapper, 
                                    IVehicle vehicleRepo, 
                                    UserManager<Classes.ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _vehicleRepo = vehicleRepo;
            _userManager = userManager;
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

            // Kollar om användaren är inloggad och om den är admin,
            // för att kunna visa admin-funktioner i vyn sen
            ViewBag.IsAdmin = User.IsInRole("Admin");

            //Hämtar async eftersom det kan ta tid att hämta data från databasen och då är det best practice?
            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id.Value); //hämtar fordonet med id genom interface -> repo -> db


            if (vehicle == null)
            {
                return NotFound();
            }

            var vehicleViewModel = _mapper.Map<VehicleViewModel>(vehicle); //mappar fordonet till VehicleViewModel

            return View("~/Views/VehicleViewModels/Details.cshtml", vehicleViewModel); //returnerar vy i trädet + detaljerna för fordonet
        }



        //***********************************************************************************************************************
        // GET: VehicleViewModels/Create

        [Authorize(Roles = "Admin")] // Endast admin kan skapa fordon
        public IActionResult Create() //ha controll för om user är admin här??
        {
            return View("~/Views/VehicleViewModels/Create.cshtml");
        }

        // POST: VehicleViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin")] // Endast admin kan skapa fordon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,PricePerDay,Description,ImageUrl1,ImageUrl2")] VehicleViewModel vehicleViewModel)
        {            
            if (ModelState.IsValid)
            {
                try
                {
                    var vehicle = _mapper.Map<Vehicle>(vehicleViewModel); //mappa om till en Vehicle
                    await _vehicleRepo.AddVehicleAsync(vehicle);
                    TempData["SuccessMessage"] = "Vehicle successfully created!";

                    return RedirectToAction("Index", "VehicleVM"); //om det funkar kommer man tillbaka till alla fordon
                }
                catch (Exception)
                {
                    TempData["SuccessMessage"] = "An error occured, please try again!";
                }

            }
            return View(vehicleViewModel); //om det inte funkar så stanna på sidan
        }

        //***********************************************************************************************************************
        // GET: VehicleViewModels/Edit/5

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id.Value);
            var vehicleViewModel = _mapper.Map<VehicleViewModel>(vehicle); //mappar fordonet till VehicleViewModel

            if (vehicleViewModel == null)
            {
                return NotFound();
            }
            return View("~/Views/VehicleViewModels/Edit.cshtml", vehicleViewModel);
        }


        // POST: VehicleViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,PricePerDay,Description,ImageUrl1,ImageUrl2")] VehicleViewModel vehicleViewModel)
        {
            if (id != vehicleViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {                    
                    
                    await _vehicleRepo.UpdateVehicleAsync(vehicleViewModel);

                    TempData["SuccessMessage"] = "Vehicle successfully updated!";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleViewModelExists(vehicleViewModel.Id))
                    {
                        return NotFound();
                    }
                    TempData["SuccessMessage"] = "An error occured, please try again!";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleViewModel);
        }


        //***********************************************************************************************************************
        // GET: VehicleViewModels/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id.Value); //hämtar fordonet med id genom interface -> repo -> db
            var vehicleViewModel = _mapper.Map<VehicleViewModel>(vehicle); //mappar fordonet till VehicleViewModel

            if (vehicleViewModel == null)
            {
                return NotFound();
            }

            return View("~/Views/VehicleViewModels/Delete.cshtml", vehicleViewModel);
        }

        // POST: VehicleViewModels/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            try
            {
                var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id); //hämtar fordonet med id genom interface -> repo -> db
                if (vehicle == null)
                {
                    return NotFound();
                }
                await _vehicleRepo.DeleteVehicleAsync(id); //anropar repo för att radera fordonet
                TempData["SuccessMessage"] = "Vehicle successfully deleted"; //skickar med en notis att fordonet är raderat
                return RedirectToAction("Index", "VehicleVM");
            }
            catch (Exception)
            {
                var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id);
                var vehicleVM = _mapper.Map<BookingViewModel>(vehicle);         //mappa om till VM

                var errorViewModel = new ErrorViewModel(); //den vill tydligen ha en sån när man skickar till Error-vyn
                if (vehicleVM == null)
                {
                    // Om nått är megatokigt – visa en generell felvy
                    return View("Error", errorViewModel);
                }
                TempData["SuccessMessage"] = "An error occured, please try again!";
                return View("Delete", vehicleVM);
            }
        }


//***********************************************************************************************************************
private bool VehicleViewModelExists(int id)
        {
            return _context.VehicleSet.Any(e => e.Id == id);
        }
    }
}
