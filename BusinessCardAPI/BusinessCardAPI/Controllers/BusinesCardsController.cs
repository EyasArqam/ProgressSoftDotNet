using BusinessCardAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinesCardsController : ControllerBase
    {
        private readonly BusinessCardDBContext _context;

        public BusinesCardsController(BusinessCardDBContext businessCardDBContext) 
        {
            this._context = businessCardDBContext;
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


    }
}
