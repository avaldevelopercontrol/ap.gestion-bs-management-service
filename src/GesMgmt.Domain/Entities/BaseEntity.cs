
namespace GesMgmt.Domain.Entities
{
    public abstract class BaseEntity
    {
        public string? UserCreated { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UserUpdated { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
