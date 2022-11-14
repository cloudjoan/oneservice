using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class F4301codetail
    {
        public string CompanyId { get; set; } = null!;
        public string? InternalType { get; set; }
        public string InternalNo { get; set; } = null!;
        public string? InternalExp { get; set; }
        public string? CostCenter { get; set; }
        public string? WorkDateS { get; set; }
        public string? WorkDateE { get; set; }
        public string? ApproveDate { get; set; }
        public string? Applicant { get; set; }
        public string? ApplyDate { get; set; }
        public string? CostFormNo { get; set; }
        public string? CrmOppNo { get; set; }
        public string? OldInternalNo { get; set; }
        public decimal? OrderCost { get; set; }
        public string? FdeptId { get; set; }
        public decimal? SapPurchasePrice { get; set; }
        public decimal? TotalPurchasePrice { get; set; }
        public decimal? SurplusBudget { get; set; }
        public string? ImportDate { get; set; }
    }
}
