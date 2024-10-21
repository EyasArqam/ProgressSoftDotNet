using BusinessCardAPI.Models.DTOs;

namespace BusinessCardAPI
{
    public class Result
    {
        public bool Ok { get; set; }
        public List<BusinessCardDTO>? Data { get; set; } = null;
        public string Message { get; set; } = "";
    }
}