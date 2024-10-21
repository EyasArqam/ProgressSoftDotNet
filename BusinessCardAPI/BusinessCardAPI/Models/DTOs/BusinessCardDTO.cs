﻿

namespace BusinessCardAPI.Models.DTOs
{
    public class BusinessCardDTO
    {

        public string Name { get; set; }
        
        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

    }
}