using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class MartAnalyseFunnelBacklogRevenue
    {
        public string? DeptId { get; set; }
        public string? Comp { get; set; }
        public string? DeptCenter { get; set; }
        public string? DeptDivision { get; set; }
        public string? DeptDepartment { get; set; }
        public string? DeptSection { get; set; }
        public string? ProfitCenter { get; set; }
        public string? Employee { get; set; }
        public string Phase { get; set; } = null!;
        public string DocNo { get; set; } = null!;
        public string? InternalNo { get; set; }
        public string? Project { get; set; }
        public string? PhasePercent { get; set; }
        public string? PhaseBl { get; set; }
        public string? RecordType { get; set; }
        public string? Account { get; set; }
        public string? Sotype { get; set; }
        public string? IndustryApplyId { get; set; }
        public string? IndustryApplyIndustry { get; set; }
        public string? IndustryApplySolution { get; set; }
        public string? IndustryApplyFunction { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? YearMonth { get; set; }
        public string? YearMonthOrderDate { get; set; }
        public string? CustomerId { get; set; }
        public string? Customer { get; set; }
        public string? Industry { get; set; }
        public string? Patype { get; set; }
        public string? Pa { get; set; }
        public string? Paindustry { get; set; }
        public string? CustomerIdsales { get; set; }
        public string? CustomerSales { get; set; }
        public string? IndustrySales { get; set; }
        public string? Location { get; set; }
        public string? ProdId { get; set; }
        public string? ProdHierarchy { get; set; }
        public string? ProdHierarchyBrad { get; set; }
        public string? DeptAnnualTargetPk { get; set; }
        public string? EmpAnnualTargetPk { get; set; }
        public string? PmtargetPk { get; set; }
        public string? EmpAnnualOrderDateTargetPk { get; set; }
        public int? FunnelAmount { get; set; }
        public int? Funnel { get; set; }
        public int? Funnel10 { get; set; }
        public int? Funnel25 { get; set; }
        public int? Funnel50 { get; set; }
        public int? Funnel75 { get; set; }
        public int? Funnel90 { get; set; }
        public int? Funnel10Profit { get; set; }
        public int? Funnel25Profit { get; set; }
        public int? Funnel50Profit { get; set; }
        public int? Funnel75Profit { get; set; }
        public int? Funnel90Profit { get; set; }
        public int? FunnelProfit { get; set; }
        public int? FunnelItemAmount { get; set; }
        public int? Quantity { get; set; }
        public int? Blamount { get; set; }
        public decimal? Soamount { get; set; }
        public decimal? Soprofit { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? Profit { get; set; }
        public decimal? OrderRevenue { get; set; }
        public float? PreOrderProfit { get; set; }
        public float? Internal15Cost { get; set; }
        public decimal? Internal15CostPs { get; set; }
        public decimal? Internal15CostSwdev { get; set; }
        public decimal? Internal15CostSwmas { get; set; }
        public decimal? Internal15CosttiCc { get; set; }
        public decimal? OrderProfit { get; set; }
        public decimal? NextBlamount { get; set; }
        public decimal? PreviousRevenue { get; set; }
        public decimal? PreviousProfit { get; set; }
        public int? NextFunnel10 { get; set; }
        public int? NextFunnel25 { get; set; }
        public int? NextFunnel50 { get; set; }
        public int? NextFunnel75 { get; set; }
        public int? NextFunnel90 { get; set; }
        public string? ProjectType { get; set; }
        public string? DealSize { get; set; }
        public DateTime? InsertTime { get; set; }
        public Guid Id { get; set; }
        public int? FunnelCommit { get; set; }
        public int? InvoiceCommit { get; set; }
    }
}
