using System;

namespace AbuAshraf.Domain.Entities
{
    /// <summary>
    /// Data plan entity for displaying available data bundles
    /// </summary>
    public class DataPlan
    {
        public Guid DataPlanId { get; set; }
        public string Network { get; set; } // MTN, Airtel, Glo, 9mobile
        public string PlanName { get; set; }
        public string DataSize { get; set; } // 1GB, 2GB, etc.
        public decimal ApiCost { get; set; }
        public decimal AdminSellingPrice { get; set; }
        public decimal AgentCost { get; set; }
        public decimal CustomerPrice { get; set; }
        public int ValidityDays { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
