using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public abstract class Entity
    {
        public Entity() 
        { 
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        [Key]
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        // TODO: This used to be SubmittedBy in name
        public DateTime CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
