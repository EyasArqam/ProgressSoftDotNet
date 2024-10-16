using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessCardAPI.Models.Entities
{
    public class Audit
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ModifiedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}
