using BusinessCardAPI.Data;
using BusinessCardAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BusinessCardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinesCardsController : ControllerBase
    {
        private readonly BusinessCardDBContext _context;
        private readonly IBusinessCardService _businessCardService;

        public BusinesCardsController(BusinessCardDBContext businessCardDBContext, IBusinessCardService businessCardService) 
        {
            this._context = businessCardDBContext;
            this._businessCardService = businessCardService;
        }

        [HttpGet("GetAllBusinessCards")]
        public async Task<IActionResult> GetAll()
        {
            var businessCards = _context.BusinessCards.AsNoTracking().Where(x => !x.IsDeleted).ToList();

            if (!businessCards.Any())
            {
                return NoContent();
            }

            return Ok(businessCards);
        }

        [HttpPost("PostFiles")]
        public async Task<IActionResult> PostFiles([FromForm] IFormFile file)
        {
            
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var businessCardDTOs = await _businessCardService.ReadBusinessCardsFromXml(file);

                if (!businessCardDTOs.Ok)
                {
                    return BadRequest(businessCardDTOs.Message);
                }

                _context.AddRangeAsync(businessCardDTOs.Data);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Business cards processed successfully.", records = businessCardDTOs.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
