namespace YorubaOrganization.Core.Entities
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

        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
