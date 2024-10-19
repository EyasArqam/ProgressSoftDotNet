using BusinessCardAPI.Interfaces;
using BusinessCardAPI.Models.DTOs;
using BusinessCardAPI.Models.Enums;
using System.Net;
using System.Numerics;
using System.Reflection;
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

                    var gender = 0;
                    if (!string.IsNullOrEmpty((string)recordElement.Element("gender")))
                    {
                        var genderString = (string)recordElement.Element("gender");
                        if (genderString.ToLower() == "female")
                        {
                            gender = 1;
                        }
                        else
                        {
                            gender = 0;
                        }
                    }

                    var record = new BusinessCardDTO
                    {
                        Name = (string?)recordElement.Element("name") ?? "",
                        Gender = gender, 
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
    
    }
}
