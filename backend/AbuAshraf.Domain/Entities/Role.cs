using System.Collections.Generic;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Role entity for role-based access control
    /// </summary>
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
