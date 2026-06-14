using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Agent entity for reseller/agent management
    /// </summary>
    public class Agent
    {
        public Guid AgentId { get; set; }
        public Guid UserId { get; set; }
        public decimal CommissionRate { get; set; } = 2.5m; // Percentage
        public decimal TotalSales { get; set; }
        public decimal TotalCommission { get; set; }
        public string AgentTier { get; set; } = "Bronze"; // Bronze, Silver, Gold, Platinum
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
