﻿using BusinessCardAPI.Models.DTOs;

namespace BusinessCardAPI.Interfaces
{
    public interface IBusinessCardService
    {
        Task<Result> ReadBusinessCardsFromXml(IFormFile file);
        Task<Result> ReadBusinessCardsFromCsv(IFormFile file);
        string ConvertToCsv(businessCard businessCard);
    }
}
