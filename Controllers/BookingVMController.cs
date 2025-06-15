using assignment_mvc_carrental.Classes;
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
    public class BookingVMController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IBooking _bookingRepo;
        private readonly IVehicle _vehicleRepo;


        public BookingVMController(ApplicationDbContext context, IMapper mapper, IBooking bookingRepo, IVehicle vehicleRep)
        {
            _context = context;
            _mapper = mapper;
            _bookingRepo = bookingRepo;
            _vehicleRepo = vehicleRep;
        }


//***********************************************************************************************************************

        // GET: BookingVM
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync(); //hämtar alla från repo
            var bookingVMList = _mapper.Map<List<BookingViewModel>>(bookings);
            return View("~/Views/BookingVM/Index.cshtml", bookingVMList);
        }



//***********************************************************************************************************************


        // GET: BookingVM/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingViewModel = await _bookingRepo.GetBookingByIDAsync(id.Value);

            if (bookingViewModel == null)
            {
                return NotFound();
            }

            return View(bookingViewModel);
        }



 //***********************************************************************************************************************

        // GET: BookingVM/Create
        public async Task<IActionResult> Create(int? vehicleId)
        {
            var vehicles = await _vehicleRepo.GetAllVehiclesAsync(); //hämtar alla fordon från databasen genom interface -> repo -> db
            var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles); //mappar fordonen till en lista av VehicleViewModel

            ViewBag.VehicleList = vehicleVMList; //skickar med fordonen till vyn som en ViewBag

            ViewBag.SelectedVehicleId = vehicleId;            // kan vara null om inget skickas med

            return View();
        }

        // POST: BookingVM/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VehicleId,CustomerId,StartDate,EndDate")] BookingViewModel bookingViewModel)
        {
            if (ModelState.IsValid)
            {
                var booking = _mapper.Map<Booking>(bookingViewModel); //mappa om till en Booking
                await _bookingRepo.AddBookingAsync(booking); //skicka till repot
                TempData["SuccessMessage"] = "Reservation successfully created!";


                return RedirectToAction("~/Views/BookingVM/Index.cshtml"); //om det funkar kommer man till alla bokningar igen
            }
            TempData["ErrorMessage"] = "An error occurred while creating the reservation. Please try again.";
            return View(bookingViewModel); //om det inte funkar stannar man på createsidan
        }



//***********************************************************************************************************************

        // GET: BookingVM/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //hämtar alla fordon för att ha dropdown-meny från databasen genom interface -> repo -> db
            var vehicles = await _vehicleRepo.GetAllVehiclesAsync(); 
            var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles); //mappar fordonen till en lista av VehicleViewModel

            ViewBag.VehicleList = vehicleVMList; //skickar med VM-fordonslista till vyn som en ViewBag
            ViewBag.SelectedVehicleId = id;      // skickar med id på förvalt fordon


            if (id == null)
            {
                return NotFound();
            }

            var bookingViewModel = await _bookingRepo.GetBookingByIDAsync(id.Value);
            if (bookingViewModel == null)
            {
                return NotFound();
            }
            return View(bookingViewModel);
        }


        // POST: BookingVM/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]          //DENNA MÅSTE FIXAS SEN
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleId,CustomerId,StartDate,EndDate,TotalPrice")] BookingViewModel bookingViewModel)
        {
            if (id != bookingViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _bookingRepo.UpdateBookingAsync(id);
                    TempData["SuccessMessage"] = "Reservation successfully updated!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingViewModelExists(bookingViewModel.Id))
                    {
                        return NotFound();
                    }

                    TempData["ErrorMessage"] = "An error occurred while editing the reservation.";
                    return View(bookingViewModel); // stanna kvar på edit-sidan
                }
            }

            return View(bookingViewModel); // ModelState is not valid
        }
       



//***********************************************************************************************************************

        // GET: BookingVM/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingViewModel = await _bookingRepo.GetBookingByIDAsync(id.Value);

            if (bookingViewModel == null)
            {
                return NotFound();
            }

            return View(bookingViewModel);
        }

        // POST: BookingVM/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {            
            try
            {
                await _bookingRepo.DeleteBookingAsync(bookingId);
                TempData["SuccessMessage"] = "Reservation successfully deleted"; //skickar med en notis att bokningen är raderad

                return RedirectToAction("Index", "VehicleVM");
            }
            catch (Exception)
            {
                //försök hitta bokningen igen så vi kan behålla rätt objekt i deletevyn när den laddas om
                var booking = await _bookingRepo.GetBookingByIDAsync(bookingId); 
                var bookingVM = _mapper.Map<BookingViewModel>(booking);         //mappa om till VM


                var errorViewModel = new ErrorViewModel(); //den vill tydligen ha en sån när man skickar till Error-vyn

                if (bookingVM == null)
                {
                    // Om nått är megatokigt – visa en generell felvy
                    return View("Error", errorViewModel); 
                }
                // Annars – visa Delete-vyn igen med bokningen kvar
                TempData["ErrorMessage"] = "Could not delete the reservation. Try again.";

                return View("Delete", bookingVM);
            }
        }




//***********************************************************************************************************************

        private bool BookingViewModelExists(int id)
        {
            return _context.BookingSet.Any(e => e.Id == id);
        }
    }
}
