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
    [Authorize]
    public class BookingVMController : Controller
    {
        private readonly IBooking _bookingRepo;
        private readonly IVehicle _vehicleRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingVMController(IBooking bookingRepo, IVehicle vehicleRepo, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _bookingRepo = bookingRepo;
            _vehicleRepo = vehicleRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: BookingVM
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync();
            var bookingVMs = _mapper.Map<List<BookingViewModel>>(bookings);
            return View(bookingVMs);
        }

        // GET: BookingVM/Create
        public async Task<IActionResult> Create(int? vehicleId)
        {
            var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
            ViewBag.VehicleList = new SelectList(vehicles, "Id", "Title", vehicleId);
            return View();
        }

        // POST: BookingVM/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
                ViewBag.VehicleList = new SelectList(vehicles, "Id", "Title", vm.VehicleId);
                TempData["ErrorMessage"] = "Something went wrong. Please check your input.";
                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var booking = _mapper.Map<Booking>(vm);
            booking.ApplicationUserId = userId;

            await _bookingRepo.AddBookingAsync(booking);
            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: BookingVM/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _bookingRepo.GetBookingByIdAsync(id.Value);
            if (booking == null) return NotFound();

            var vm = _mapper.Map<BookingViewModel>(booking);

            var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
            ViewBag.VehicleList = new SelectList(vehicles, "Id", "Title", vm.VehicleId);

            return View(vm);
        }

        // POST: BookingVM/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var vehicles = await _vehicleRepo.GetAllVehiclesAsync();
                ViewBag.VehicleList = new SelectList(vehicles, "Id", "Title", vm.VehicleId);
                return View(vm);
            }

            try
            {
                var updatedBooking = _mapper.Map<Booking>(vm);
                await _bookingRepo.UpdateBookingAsync(updatedBooking);
                TempData["SuccessMessage"] = "Booking updated!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Something went wrong while updating.";
                return View(vm);
            }
        }

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
                TempData["SuccessMessage"] = "Booking deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "Could not delete booking.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
