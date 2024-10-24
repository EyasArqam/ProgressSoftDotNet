﻿using AutoMapper;
using BusinessCardAPI.Data;
using BusinessCardAPI.Interfaces;
using BusinessCardAPI.Models.DTOs;
using BusinessCardAPI.Models.Entities;
using BusinessCardAPI.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Serialization;

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

        [HttpGet("GetFilteredBusinessCards")]
        public async Task<IActionResult> GetFilteredBusinessCards([FromQuery] Search? searchParams)
        {
            var businessCards = _context.BusinessCards.AsNoTracking().Where(x => !x.IsDeleted);

            if (!businessCards.Any())
            {
                return NoContent();
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Name))
            {
                businessCards = businessCards.Where(bc => bc.Name.Contains(searchParams.Name));
            }

            if (searchParams.Gender is not null)
            {
                businessCards = businessCards.Where(bc => bc.Gender == searchParams.Gender);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Email))
            {
                businessCards = businessCards.Where(bc => bc.Email == searchParams.Email);
            }

            if (!string.IsNullOrWhiteSpace(searchParams.Phone))
            {
                businessCards = businessCards.Where(bc => bc.Phone == searchParams.Phone);
            }

            if (searchParams.DOB.HasValue)
            {
                var date = searchParams.DOB.Value.Date;
  
                businessCards = businessCards.Where(bc =>
                                bc.DateOfBirth.HasValue &&
                                bc.DateOfBirth.Value.Year == date.Year &&
                                bc.DateOfBirth.Value.Month == date.Month &&
                                bc.DateOfBirth.Value.Day == date.Day
                            );
            }

            return Ok(await businessCards.ToListAsync());
        }

        [HttpPost("PostXmlFile")]
        public async Task<IActionResult> PostXmlFile([FromForm] IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(((int)ErrorCode.NoFile).ToString(), "No file uploaded.");
                return BadRequest(ModelState);
            }

            try
            {

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (fileExtension != ".xml")
                {
                    ModelState.AddModelError(((int)ErrorCode.UnsupportedFile).ToString(), "Unsupported file type. Please upload an XML file.");
                    return BadRequest(ModelState);
                }


                List<BusinessCard> businessCards = new List<BusinessCard>();

                var businessCardDTOs = await _businessCardService.ReadBusinessCardsFromXml(file);

                if (!businessCardDTOs.Ok)
                {
                    return BadRequest(businessCardDTOs.Message);
                }

                foreach (var item in businessCardDTOs.Data)
                {
                    BusinessCard businessCard = _mapper.Map<BusinessCard>(item);
                    businessCards.Add(businessCard);
                }


                await _context.BusinessCards.AddRangeAsync(businessCards);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Business cards processed successfully.", records = businessCards });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("PostCsvFile")]
        public async Task<IActionResult> PostCsvFile([FromForm] IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(((int)ErrorCode.NoFile).ToString(), "No file uploaded.");
                return BadRequest(ModelState);
            }

            try
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (fileExtension != ".csv")
                {
                    ModelState.AddModelError(((int)ErrorCode.UnsupportedFile).ToString(), "Unsupported file type. Please upload an CSV file.");
                    return BadRequest(ModelState);
                }

                List<BusinessCard> businessCards = new List<BusinessCard>();

                var businessCardDTOs = await _businessCardService.ReadBusinessCardsFromCsv(file);

                if (!businessCardDTOs.Ok)
                {
                    return BadRequest(businessCardDTOs.Message);
                }

                foreach (var item in businessCardDTOs.Data)
                {
                    BusinessCard businessCard = _mapper.Map<BusinessCard>(item);
                    businessCards.Add(businessCard);
                }


                await _context.BusinessCards.AddRangeAsync(businessCards);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Business cards processed successfully.", records = businessCards });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("PostForm")]
        public async Task<IActionResult> PostForm([FromBody] BusinessCard formData)
        {


            if (formData == null)
            {
                ModelState.AddModelError(((int)ErrorCode.FormIsNull).ToString(), "Form data cannot be null.");
                return BadRequest(ModelState);
            }

            try
            {
                await _context.BusinessCards.AddAsync(formData);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Business card processed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding business Card.");
            }
        }

        [HttpDelete("DeleteBusinessCard/{id}")]
        public async Task<IActionResult> DeleteBusinessCard(int id)
        {
            var businessCard = await _context.BusinessCards.FindAsync(id);

            if (businessCard == null)
            {
                ModelState.AddModelError(((int)ErrorCode.NotFound).ToString(), "No business card found with the specified ID.");
                return NotFound(ModelState);
            }

            businessCard.IsDeleted = true;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Business card deleted successfully." });
        }

        [HttpGet("ExportXml/{id}")]
        public async Task<IActionResult> ExportXml(int id)
        {
            var findBusinessCard = await _context.BusinessCards.FindAsync(id);

            if (findBusinessCard == null)
            {
                return NotFound();
            }

            try
            {
                businessCard businessCard = _mapper.Map<businessCard>(findBusinessCard);


                var serializer = new XmlSerializer(typeof(businessCard));
                using var stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, businessCard);
                var xmlContent = stringWriter.ToString();

                var bytes = Encoding.UTF8.GetBytes(xmlContent);

                return File(bytes, "application/xml", $"businessCard_{id}.xml");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("ExportCsv/{id}")]
        public async Task<IActionResult> ExportCsv(int id)
        {
            var findBusinessCard = await _context.BusinessCards.FindAsync(id);

            if (findBusinessCard == null)
            {
                return NotFound();
            }

            try
            {
                businessCard businessCard = _mapper.Map<businessCard>(findBusinessCard);

                var csvContent = _businessCardService.ConvertToCsv(businessCard);

                var bytes = Encoding.UTF8.GetBytes(csvContent);

                return File(bytes, "text/csv", $"BusinessCard_{id}.csv");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ListName")]
        public async Task<IActionResult> GetListOfName([FromQuery] string? searchTerm)
        {
            var businessCardsQuery = _context.BusinessCards.AsNoTracking().Where(bc => !bc.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                businessCardsQuery = businessCardsQuery.Where(bc => bc.Name.Contains(searchTerm));
            }

            var businessCards = await businessCardsQuery.Select(bc => bc.Name).ToListAsync();

            if (businessCards.Count == 0)
            {
                return NoContent();
            }

            return Ok(businessCards);
        }

        [HttpGet("ListPhone")]
        public async Task<IActionResult> GetListOfPhone([FromQuery] string? searchTerm)
        {
            var businessCardsQuery = _context.BusinessCards.AsNoTracking().Where(bc => !bc.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                businessCardsQuery = businessCardsQuery.Where(bc => bc.Phone == searchTerm);
            }

            var businessCards = await businessCardsQuery.Select(bc => bc.Phone).ToListAsync();

            if (businessCards.Count == 0)
            {
                return NoContent();
            }

            return Ok(businessCards);
        }

        [HttpGet("ListEmail")]
        public async Task<IActionResult> GetListOfEmail([FromQuery] string? searchTerm)
        {
            var businessCardsQuery = _context.BusinessCards.AsNoTracking().Where(bc => !bc.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                businessCardsQuery = businessCardsQuery.Where(bc => bc.Email == searchTerm);
            }

            var businessCards = await businessCardsQuery.Select(bc => bc.Email).ToListAsync();

            if (businessCards.Count == 0)
            {
                return NoContent();
            }

            return Ok(businessCards);
        }

    }

}
