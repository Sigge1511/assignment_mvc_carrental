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
using System.Security.Claims;

namespace assignment_mvc_carrental.Controllers
{
    public class BookingVMController : Controller
    {
        private readonly IBooking _bookingRepo;
        private readonly IVehicle _vehicleRepo;
        private readonly IApplicationUser _userRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingVMController(IBooking bookingRepo, IVehicle vehicleRepo, IApplicationUser appuserrepo, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _bookingRepo = bookingRepo;
            _vehicleRepo = vehicleRepo;
            _userRepo = appuserrepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        //***********************************************************************************************************************

        // GET: BookingVM
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync();
            var bookingVMs = _mapper.Map<List<BookingViewModel>>(bookings);
            return View(bookingVMs);
        }


//***********************************************************************************************************************
        //NÄR EN ANVÄNDARE SKAPAR EN NY BOKNING 

        // GET: BookingVM/Create
        public async Task<IActionResult> Create(int? vehicleId)
        {
            var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
            var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles);

            ViewBag.VehicleList = vehicleVMList;
            ViewBag.SelectedVehicleId = vehicleId;
            return View();
        }

        // POST: BookingVM/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel vm)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (vm.StartDate < today) //datumkoll
            {
                ModelState.AddModelError(nameof(vm.StartDate), "Start date cannot be in the past.");
            }

            var days = (vm.EndDate.DayNumber - vm.StartDate.DayNumber) + 1; //mer datumkoll ang slutdatum
            if (days < 1)
            {
                ModelState.AddModelError(nameof(vm.EndDate), "End date must be the same or after the start date.");
            }

            if (!ModelState.IsValid) //om något är tokigt så stanna i vyn med valda fordonet
            {
                var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
                var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles);
                ViewBag.VehicleList = vehicleVMList;
                ViewBag.SelectedVehicleId = vm.VehicleId;

                TempData["ErrorMessage"] = "Something went wrong. Please check your input.";
                return View(vm);
            }

            //.....................................................................................

            //errorhantering om något går tokigt med user eller fordon
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(vm.VehicleId);
            if (vehicle == null) return NotFound();

            //.....................................................................................

            var totalPrice = vehicle.PricePerDay * days; //räkna ut totalpris för bokning

            var booking = _mapper.Map<Booking>(vm); //skapa en ny bokning mha mappning

            //sätt de viktiga värdena för user och fordon till min bokning
            booking.ApplicationUserId = userId;
            booking.TotalPrice = totalPrice;

            //Ropa på mitt repo och skicka med bokningen
            try
            {
                await _bookingRepo.AddBookingAsync(booking);
                TempData["SuccessMessage"] = "Booking created successfully!";

                //om allt gått bra och bokningen skapats skickas anv till fordonslistan igen
                return RedirectToAction("Index", "VehicleVM");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong while saving your booking. Please try again.";

                var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
                var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles);
                ViewBag.VehicleList = vehicleVMList;
                ViewBag.SelectedVehicleId = vm.VehicleId;

                //fixat all data som behövs igen för att återgå till vyn vid error
                return View(vm);
            }
        }

//***********************************************************************************************************************
        //NÄR EN ADMIN SKAPAR EN NY BOKNING 

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AdminCreate()
        {
            var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
            var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles);
            ViewBag.VehicleList = vehicleVMList;

            var customers = await _userManager.GetUsersInRoleAsync("Customer"); // <-- RÄTT SÄTT!
            var customerVMList = _mapper.Map<List<CustomerViewModel>>(customers);
            ViewBag.CustomerList = customerVMList;

            return View("AdminCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminCreate(BookingViewModel vm)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (vm.StartDate < today)
            {
                ModelState.AddModelError(nameof(vm.StartDate), "Start date cannot be in the past.");
            }

            var days = (vm.EndDate.DayNumber - vm.StartDate.DayNumber) + 1;
            if (days < 1)
            {
                ModelState.AddModelError(nameof(vm.EndDate), "End date must be the same or after the start date.");
            }

            if (!ModelState.IsValid)
            {
                var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
                ViewBag.VehicleList = _mapper.Map<List<VehicleViewModel>>(vehicles);

                var customers = await _userManager.GetUsersInRoleAsync("Customer");
                ViewBag.CustomerList = _mapper.Map<List<CustomerViewModel>>(customers);

                TempData["ErrorMessage"] = "Something went wrong. Please check your input.";
                return View(vm);
            }

            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(vm.VehicleId);
            if (vehicle == null)
            {
                TempData["ErrorMessage"] = "Selected vehicle not found.";
                return RedirectToAction("Index");
            }

            var totalPrice = vehicle.PricePerDay * days;

            var booking = _mapper.Map<Booking>(vm);
            booking.TotalPrice = totalPrice;

            try
            {
                await _bookingRepo.AddBookingAsync(booking);
                TempData["SuccessMessage"] = "Booking successfully created.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Something went wrong while saving the booking.";

                var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
                ViewBag.VehicleList = _mapper.Map<List<VehicleViewModel>>(vehicles);

                var customers = await _userManager.GetUsersInRoleAsync("Customer");
                ViewBag.CustomerList = _mapper.Map<List<CustomerViewModel>>(customers);

                return View(vm);
            }
        }

        //***********************************************************************************************************************

        // GET: BookingVM/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _bookingRepo.GetBookingByIdAsync(id.Value);
            if (booking == null) return NotFound();

            var vm = _mapper.Map<BookingViewModel>(booking);

            var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
            var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles);

            ViewBag.VehicleList = vehicleVMList;
            ViewBag.SelectedVehicleId = vm.VehicleId;

            return View(vm);
        }

        // POST: BookingVM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingViewModel vm)
        {
            //errorskydd/kollar
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
                var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles);
                ViewBag.VehicleList = vehicleVMList;
                ViewBag.SelectedVehicleId = vm.VehicleId;
                return View(vm);
            }
            //***************************************************************************
            try
            {
                //Hämta bokningen från DB
                var existingBooking = await _bookingRepo.GetBookingByIdAsync(id);
                if (existingBooking == null) return NotFound();

                // Uppdatera tillåtna fält
                existingBooking.VehicleId = vm.VehicleId;
                existingBooking.StartDate = vm.StartDate;
                existingBooking.EndDate = vm.EndDate;

                // Räkna om priset
                var vehicle = await _vehicleRepo.GetVehicleByIDAsync(vm.VehicleId);
                var days = (vm.EndDate.DayNumber - vm.StartDate.DayNumber) + 1;
                existingBooking.TotalPrice = vehicle.PricePerDay * days;

                //Spara uppdaterad bokning
                await _bookingRepo.UpdateBookingAsync(existingBooking);

                TempData["SuccessMessage"] = "Booking updated!";
                return RedirectToAction("Index", "VehicleVM");
            }
            catch 
            {
                TempData["ErrorMessage"] = "Something went wrong while updating.";
                return View(vm);
            }
        }

//***********************************************************************************************************************

        // GET: BookingVM/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _bookingRepo.GetBookingByIdAsync(id.Value);
            if (booking == null) return NotFound();

            var vm = _mapper.Map<BookingViewModel>(booking);
            return View(vm);
        }

        // POST: BookingVM/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _bookingRepo.DeleteBookingAsync(id);
                TempData["SuccessMessage"] = "Reservation deleted.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not delete reservation.";
            }

            return View("Index", "Home"); 
        }
    }
}
