using AutoMapper;
using BusinessCardAPI.Data;
using BusinessCardAPI.Interfaces;
using BusinessCardAPI.Models.DTOs;
using BusinessCardAPI.Models.Entities;
using BusinessCardAPI.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;


namespace BusinessCardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinesCardsController : ControllerBase
    {
        private readonly BusinessCardDBContext _context;
        private readonly IBusinessCardService _businessCardService;
        private readonly IMapper _mapper;

        public BusinesCardsController(BusinessCardDBContext businessCardDBContext, IBusinessCardService businessCardService, IMapper mapper)
        {
            this._context = businessCardDBContext;
            this._businessCardService = businessCardService;
            _mapper = mapper;
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

                List<BusinessCard> businessCards = new List<BusinessCard>();
                foreach (var item in businessCardDTOs.Data)
                {
                    BusinessCard businessCard = _mapper.Map<BusinessCard>((BusinessCardDTO)item);

                    businessCards.Add(businessCard);
                }

                _context.BusinessCards.AddRangeAsync(businessCards);

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
