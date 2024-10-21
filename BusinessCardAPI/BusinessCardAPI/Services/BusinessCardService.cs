using BusinessCardAPI.Interfaces;
using BusinessCardAPI.Models.DTOs;
using BusinessCardAPI.Models.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using System.Formats.Asn1;
using System.Globalization;
using System.Xml.Linq;

namespace BusinessCardAPI.Services
{
    public class BusinessCardService : IBusinessCardService
    {
        public async Task<Result> ReadBusinessCardsFromXml(IFormFile file)
        {
            var businessCardDTOs = new List<BusinessCardDTO>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream); 
                stream.Position = 0;

                var xmlDocument = XDocument.Load(stream);

                foreach (var recordElement in xmlDocument.Descendants("businessCard"))
                {


                    var record = new BusinessCardDTO
                    {
                        Name = (string?)recordElement.Element("name") ?? "",
                        Gender = (string)recordElement.Element("gender") ?? "Male", 
                        Email = (string?)recordElement.Element("email") ?? "",
                        DateOfBirth = (DateTime?)recordElement.Element("dateOfBirth") ?? null,
                        Address = (string?)recordElement.Element("address") ?? "",
                        Phone = (string?)recordElement.Element("phone") ?? ""
                    };

                    if (string.IsNullOrEmpty(record.Name) || string.IsNullOrEmpty(record.Email) || string.IsNullOrEmpty(record.Phone))
                    {
                        throw new InvalidDataException($"Invalid data: Name, Email, and Phone are required for each business card.");
                    }

                    businessCardDTOs.Add(record);
                }
            }

            return new Result 
            { 
                Ok = true, 
                Data = businessCardDTOs,
            };
        }

        public async Task<Result> ReadBusinessCardsFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new Result { Ok = false, Message = "Invalid file." };
            }

            var businessCards = new List<BusinessCardDTO>();

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
                {
                    businessCards = csv.GetRecords<BusinessCardDTO>()
                        .Select(record => new BusinessCardDTO
                        {
                            Name = record.Name,
                            Gender = record.Gender,
                            DateOfBirth = record.DateOfBirth,
                            Email = record.Email,
                            Phone = record.Phone,
                            Address = record.Address
                        }).ToList();
                }

                return new Result
                {
                    Ok = true,
                    Message = "File read successfully.",
                    Data = businessCards
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Ok = false,
                    Message = $"Error reading file: {ex.Message}"
                };
            }
        }


    }
}
