using BusinessCardAPI.Models.Enums;

namespace BusinessCardAPI
{
    public class Search
    {
        public string? Name { get; set; }
        public Gender? Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DOB { get; set; }
    }
}