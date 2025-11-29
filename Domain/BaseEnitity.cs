namespace Inventory.Api.Domain
{
    public abstract class BaseEntity
    {
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } 
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
