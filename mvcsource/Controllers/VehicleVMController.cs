using assignment_mvc_carrental.Models;
using assignment_mvc_carrental.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace assignment_mvc_carrental.Controllers
{
    public class VehicleVMController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;

        //För göra risken mindre för strul med deserialisering
        private readonly JsonSerializerOptions _jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public VehicleVMController(IMapper mapper, HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
        }
//***********************************************************************************************************************
//Visa ALLA FORDON
// GET: VehicleVMController/Index
[Route("allvehicles")]
public async Task<IActionResult> Index()
{
    try
    {
        // Use the named HttpClient with proper base address
        var client = _httpClientFactory.CreateClient("CarRentalAPI");

        var response = await client.GetAsync("api/VehicleDt");
        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = $"Misslyckades hämta fordon från API. Status: {response.StatusCode}";
            return View("~/Views/VehicleViewModels/Index.cshtml", new List<VehicleViewModel>());
        }

        var stream = await response.Content.ReadAsStreamAsync();
        var vehiclesList = await JsonSerializer.DeserializeAsync<List<VehicleViewModel>>(stream, _jsonOptions)
                          ?? new List<VehicleViewModel>();

        return View("~/Views/VehicleViewModels/Index.cshtml", vehiclesList);
    }
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = $"An error occured: ({ex.Message})";
        return View("~/Views/VehicleViewModels/Index.cshtml", new List<VehicleViewModel>());
    }
}

//***********************************************************************************************************************
        // GET: VehicleVMController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var response = await client.GetAsync($"api/VehicleDt/{id.Value}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound();
                    TempData["ErrorMessage"] = "Could not find vehicle.";
                    return View("Error");
                }

                var vehicleInfo = await response.Content.ReadAsStringAsync();
                var vehicle = JsonSerializer.Deserialize<VehicleViewModel>(vehicleInfo, _jsonOptions);
                if (vehicle == null)
                {
                    return NotFound();
                }

                return View("~/Views/VehicleViewModels/Details.cshtml", vehicle);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occured.";
                return View("Error");
            }
        }
//***********************************************************************************************************************
        // GET: VehicleVMController/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View("~/Views/VehicleViewModels/Create.cshtml");
        }

        // POST: VehicleVMController/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Year,PricePerDay,Description,ImageUrl1,ImageUrl2")] VehicleViewModel vehicleViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Use the named HttpClient with proper base address
                    var client = _httpClientFactory.CreateClient("CarRentalAPI");

                    var vehicle = _mapper.Map<Vehicle>(vehicleViewModel);
                    var vehicleToJson = JsonSerializer.Serialize(vehicle);
                    var vehicleToApi = new StringContent(vehicleToJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("api/VehicleDt", vehicleToApi);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Vehicle successfully added!";
                        return RedirectToAction(nameof(Index));
                    }

                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Failed to add vehicle: {response.ReasonPhrase}. Detaljer: {errorContent}");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occured.");
                }
            }
            return View("~/Views/VehicleViewModels/Create.cshtml", vehicleViewModel);
        }

//***********************************************************************************************************************
        // GET: VehicleVMController/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var response = await client.GetAsync($"api/VehicleDt/{id.Value}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var vehicleJson = await response.Content.ReadAsStringAsync();
                var vehicle = JsonSerializer.Deserialize<VehicleViewModel>(vehicleJson, _jsonOptions);
                if (vehicle == null)
                {
                    return NotFound();
                }

                return View("~/Views/VehicleViewModels/Edit.cshtml", vehicle);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occured.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: VehicleVMController/Edit/5
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
                    // Use the named HttpClient with proper base address
                    var client = _httpClientFactory.CreateClient("CarRentalAPI");

                    var vehicle = _mapper.Map<Vehicle>(vehicleViewModel);
                    var vehicleJson = JsonSerializer.Serialize(vehicle);
                    var vehicleToApi = new StringContent(vehicleJson, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync($"api/VehicleDt/{id}", vehicleToApi);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Vehicle was updated!";
                        return RedirectToAction(nameof(Index));
                    }
                    if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong. Please try again.");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError(string.Empty, $"Unable to update: {response.ReasonPhrase}. Details: {errorContent}");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occured.");
                }
            }
            return View("~/Views/VehicleViewModels/Edit.cshtml", vehicleViewModel);
        }


        //***********************************************************************************************************************
        // GET: VehicleVMController/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var response = await client.GetAsync($"VehicleDt/{id.Value}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var vehicleInfoApi = await response.Content.ReadAsStringAsync();
                var vehicle = JsonSerializer.Deserialize<VehicleViewModel>(vehicleInfoApi, _jsonOptions);
                if (vehicle == null)
                {
                    return NotFound();
                }

                return View("~/Views/VehicleViewModels/Delete.cshtml", vehicle);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Could not fetch vehicle info.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: VehicleVMController/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Use the named HttpClient with proper base address
                var client = _httpClientFactory.CreateClient("CarRentalAPI");

                var response = await client.DeleteAsync($"VehicleDt/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Vehicle was deleted.";
                    return RedirectToAction(nameof(Index));
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Could not delete vehicle: {response.ReasonPhrase}. Details: {errorContent}");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occured.");
            }

            // Om det misslyckas, hämta fordonet IGEN för att visa/stanna i Delete-vyn med felmeddelande
            var client2 = _httpClientFactory.CreateClient("CarRentalAPI");
            var vehicleResponse = await client2.GetAsync($"VehicleDt/{id}");
            if (vehicleResponse.IsSuccessStatusCode)
            {
                var vehicleInfoApi = await vehicleResponse.Content.ReadAsStringAsync();
                var vehicle = JsonSerializer.Deserialize<VehicleViewModel>(vehicleInfoApi, _jsonOptions);
                if (vehicle == null)
                {
                    return NotFound();
                }
                return View("~/Views/VehicleViewModels/Delete.cshtml", vehicle);
            }
            return NotFound();
        }
    }
}

//***********************************************************************************************************************
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
//        // GET: VehicleViewModels
//        [Route("allvehicles")]
//        public async Task<IActionResult> Index()
//        {
//            try
//            {
//                // Create client. Prefer a named client configured in Program.cs as "ApiClient".
//                var client = _httpClientFactory.CreateClient("ApiClient");

//                // Call the API endpoint that returns a JSON array of Vehicle
//                var response = await client.GetAsync("api/vehiclesList");
//                if (!response.IsSuccessStatusCode)
//                {
//                    TempData["SuccessMessage"] = "Failed to retrieve vehiclesList from API.";
//                    return View("~/Views/VehicleViewModels/Index.cshtml", new List<VehicleViewModel>());
//                }

//                // Deserialize the JSON response into domain models
//                var stream = await response.Content.ReadAsStreamAsync(); //!
//                var jsonOptions = new System.Text.Json.JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                };
//                var vehicleVMList = await System.Text.Json.JsonSerializer.DeserializeAsync<List<VehicleViewModel>>(stream, jsonOptions)
//                               ?? new List<VehicleViewModel>();

//                // Map to view models and return the view
//                //var vehicleVMList = _mapper.Map<List<VehicleViewModel>>(vehiclesList);
//                return View("~/Views/VehicleViewModels/Index.cshtml", vehicleVMList);
//            }
//            catch (Exception)
//            {
//                TempData["SuccessMessage"] = "An error occurred while contacting the API.";
//                return View("~/Views/VehicleViewModels/Index.cshtml", new List<VehicleViewModel>());
//            }
//        }


//        //***********************************************************************************************************************
//        // GET: VehicleViewModels/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }
//            // Kollar om användaren är inloggad och om den är admin,
//            // för att kunna visa admin-funktioner i vyn 

//            //BYTA DETTA MOT KOLL MHA JWT OCH CLAIMS I STÄLLET SEDAN
//            //ViewBag.IsAdmin = User.IsInRole("Admin");

//            var client = _httpClientFactory.CreateClient("ApiClient");
//            var vehicle = await client.GetAsync("api/vehicledetails",id.Value);
//            //await _vehicleRepo.GetVehicleByIDAsync(id.Value); //hämtar fordonet med id genom interface -> repo -> db


//            if (vehicle == null)
//            {
//                return NotFound();
//            }

//            var vehicleViewModel = _mapper.Map<VehicleViewModel>(vehicle); //mappar fordonet till VehicleViewModel

//            return View("~/Views/VehicleViewModels/Details.cshtml", vehicleViewModel); //returnerar vy i trädet + detaljerna för fordonet
//        }



//        //***********************************************************************************************************************
//        // GET: VehicleViewModels/Create

//        [Authorize(Roles = "Admin")] // Endast admin kan skapa fordon
//        public IActionResult Create() //ha controll för om user är admin här??
//        {
//            return View("~/Views/VehicleViewModels/Create.cshtml");
//        }

//        // POST: VehicleViewModels/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

//        [Authorize(Roles = "Admin")] // Endast admin kan skapa fordon
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("Id,Title,Year,PricePerDay,Description,ImageUrl1,ImageUrl2")] VehicleViewModel vehicleViewModel)
//        {            
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    var vehicle = _mapper.Map<Vehicle>(vehicleViewModel); //mappa om till en Vehicle
//                    await _vehicleRepo.AddVehicleAsync(vehicle);
//                    TempData["SuccessMessage"] = "Vehicle successfully created!";

//                    return RedirectToAction("Index", "VehicleVM"); //om det funkar kommer man tillbaka till alla fordon
//                }
//                catch (Exception)
//                {
//                    TempData["SuccessMessage"] = "An error occured, please try again!";
//                }

//            }
//            return View(vehicleViewModel); //om det inte funkar så stanna på sidan
//        }

//        //***********************************************************************************************************************
//        // GET: VehicleViewModels/Edit/5

//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id.Value);
//            var vehicleViewModel = _mapper.Map<VehicleViewModel>(vehicle); //mappar fordonet till VehicleViewModel

//            if (vehicleViewModel == null)
//            {
//                return NotFound();
//            }
//            return View("~/Views/VehicleViewModels/Edit.cshtml", vehicleViewModel);
//        }


//        // POST: VehicleViewModels/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [Authorize(Roles = "Admin")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,PricePerDay,Description,ImageUrl1,ImageUrl2")] VehicleViewModel vehicleViewModel)
//        {
//            if (id != vehicleViewModel.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {                    
                    
//                    await _vehicleRepo.UpdateVehicleAsync(vehicleViewModel);

//                    TempData["SuccessMessage"] = "Vehicle successfully updated!";

//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!VehicleViewModelExists(vehicleViewModel.Id))
//                    {
//                        return NotFound();
//                    }
//                    TempData["SuccessMessage"] = "An error occured, please try again!";
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(vehicleViewModel);
//        }


//        //***********************************************************************************************************************
//        // GET: VehicleViewModels/Delete/5
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id.Value); //hämtar fordonet med id genom interface -> repo -> db
//            var vehicleViewModel = _mapper.Map<VehicleViewModel>(vehicle); //mappar fordonet till VehicleViewModel

//            if (vehicleViewModel == null)
//            {
//                return NotFound();
//            }

//            return View("~/Views/VehicleViewModels/Delete.cshtml", vehicleViewModel);
//        }

//        // POST: VehicleViewModels/Delete/5
//        [Authorize(Roles = "Admin")]
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteVehicle(int id)
//        {
//            try
//            {
//                var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id); //hämtar fordonet med id genom interface -> repo -> db
//                if (vehicle == null)
//                {
//                    return NotFound();
//                }
//                await _vehicleRepo.DeleteVehicleAsync(id); //anropar repo för att radera fordonet
//                TempData["SuccessMessage"] = "Vehicle successfully deleted"; //skickar med en notis att fordonet är raderat
//                return RedirectToAction("Index", "VehicleVM");
//            }
//            catch (Exception)
//            {
//                var vehicle = await _vehicleRepo.GetVehicleByIDAsync(id);
//                var vehicleVM = _mapper.Map<BookingViewModel>(vehicle);         //mappa om till VM

//                var errorViewModel = new ErrorViewModel(); //den vill tydligen ha en sån när man skickar till Error-vyn
//                if (vehicleVM == null)
//                {
//                    // Om nått är megatokigt – visa en generell felvy
//                    return View("Error", errorViewModel);
//                }
//                TempData["SuccessMessage"] = "An error occured, please try again!";
//                return View("Delete", vehicleVM);
//            }
//        }


////***********************************************************************************************************************
//private bool VehicleViewModelExists(int id)
//        {
//            return _context.VehicleSet.Any(e => e.Id == id);
//        }
//    }
//}
