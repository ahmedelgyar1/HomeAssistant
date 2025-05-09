using HomeAssistant.DTOs;
using HomeAssistant.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LocationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Location")]
        public async Task<IActionResult> SubmitLocation([FromBody] LocationDto location)
        {
            if (!ModelState.IsValid)
                return BadRequest("Please fill in all required fields.");

            var loc = new Location
            {
                Apartment = location.Apartment,
                StreetAddress = location.StreetAddress,
                State = location.State,
                City = location.City,
                Country = location.Country,
                ZipCode = location.ZipCode
            };

            await _context.Locations.AddAsync(loc);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Location saved successfully" });
        }
    }

}
