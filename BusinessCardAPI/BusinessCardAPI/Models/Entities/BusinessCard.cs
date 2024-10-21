using BusinessCardAPI.Attributes;
using BusinessCardAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace BusinessCardAPI.Models.Entities
{
    public class BusinessCard : Audit
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        public Gender Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(150, ErrorMessage = "Email cannot be longer than 150 characters.")]
        public string Email { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        [StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters.")]
        public string Phone { get; set; }

        [StringLength(250, ErrorMessage = "Address cannot be longer than 250 characters.")]
        public string Address { get; set; }

        [MaxBase64Size(1048576, ErrorMessage = "Photo size cannot exceed 1 MB.")]
        public string? Photo { get; set; }
    }
}
