using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public abstract class BaseEntity
    {
        public BaseEntity() 
        { 
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }

        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        // TODO: This used to be SubmittedBy in name. Ensure that API does not break
        public DateTime CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
