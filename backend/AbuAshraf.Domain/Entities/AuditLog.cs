using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Audit log entity for tracking system changes
    /// </summary>
    public class AuditLog
    {
        public Guid AuditId { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string OldValues { get; set; } // JSON
        public string NewValues { get; set; } // JSON
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
