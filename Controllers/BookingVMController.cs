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

        public BookingVMController(ApplicationDbContext context, IMapper mapper, IBooking bookingRepo)
        {
            _context = context;
            _mapper = mapper;
            _bookingRepo = bookingRepo;
        }
        //***********************************************************************************************************************

        // GET: BookingVM
        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingRepo.GetAllBookingsAsync(); //hämtar alla från repo
            var bookingVMList = _mapper.Map<List<BookingViewModel>>(bookings);
            return View("~/Views/BookingVM/Index.cshtml", bookingVMList);
        }

        //var vehicles = await _vehicleRepo.GetAllVehiclesAsync(); //hämtar alla fordon från databasen genom interface -> repo -> db
        //var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehicles); //mappar fordonen till en lista av VehicleViewModel
        //    return View("~/Views/VehicleViewModels/Index.cshtml", vehicleVMList); //returnerar VMlistan och skickar till rätt vy i trädet
        //***********************************************************************************************************************


        // GET: BookingVM/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _bookingRepo.GetBookingByIDAsync(id.Value); //hämtar bokningen med id genom interface -> repo -> db              
            if (booking == null)
            {
                return NotFound();
            }
            var bookingViewModel = new BookingViewModel
            {
                
            }; //mappar bokningen till BookingViewModel och gör om id:n till namn för kund och fordon


            return View(bookingViewModel);
        }
        //***********************************************************************************************************************

        // GET: BookingVM/Create
        public IActionResult Create()
        {            
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
                

                //************** HA NÅGOT SOM KAN TRIGGA JAVASCRIPT ALERT OM LYCKAD BOKNING???? ***************************************************

                return RedirectToAction("~/Views/BookingVM/Index.cshtml"); //om det funkar kommer man till alla bokningar igen
            }
            return View(bookingViewModel); //om det inte funkar stannar man på createsidan
        }
        //***********************************************************************************************************************

        // GET: BookingVM/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingViewModel = await _context.BookingSet.FindAsync(id);
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
        [ValidateAntiForgeryToken]
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
                    _context.Update(bookingViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingViewModelExists(bookingViewModel.Id))
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
            return View(bookingViewModel);
        }
        //***********************************************************************************************************************

        // GET: BookingVM/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingViewModel = await _context.BookingSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookingViewModel == null)
            {
                return NotFound();
            }

            return View(bookingViewModel);
        }

        // POST: BookingVM/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookingViewModel = await _context.BookingSet.FindAsync(id);
            if (bookingViewModel != null)
            {
                _context.BookingSet.Remove(bookingViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //***********************************************************************************************************************

        private bool BookingViewModelExists(int id)
        {
            return _context.BookingSet.Any(e => e.Id == id);
        }
    }
}
