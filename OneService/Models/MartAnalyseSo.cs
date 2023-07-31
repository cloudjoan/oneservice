using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class MartAnalyseSo
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
        public string Sono { get; set; } = null!;
        public string Soitem { get; set; } = null!;
        public string Sopk { get; set; } = null!;
        public string? InternalNo { get; set; }
        public string? Project { get; set; }
        public string PhasePercent { get; set; } = null!;
        public string? PhaseBl { get; set; }
        public string RecordType { get; set; } = null!;
        public string Account { get; set; } = null!;
        public string? Sotype { get; set; }
        public string? IndustryApplyId { get; set; }
        public string? IndustryApplyIndustry { get; set; }
        public string? IndustryApplySolution { get; set; }
        public string? IndustryApplyFunction { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? YearMonth { get; set; }
        public string? YearMonthOrderDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CustomerId { get; set; }
        public string? Customer { get; set; }
        public string? Industry { get; set; }
        public string Patype { get; set; } = null!;
        public string Pa { get; set; } = null!;
        public string Paindustry { get; set; } = null!;
        public string? CustomerIdsales { get; set; }
        public string? CustomerSales { get; set; }
        public string? IndustrySales { get; set; }
        public string? Location { get; set; }
        public string ShipType { get; set; } = null!;
        public string? ProdId { get; set; }
        public string? ProdHierarchy { get; set; }
        public string? ProdHierarchyBrad { get; set; }
        public string? DeptAnnualTargetPk { get; set; }
        public string? EmpAnnualTargetPk { get; set; }
        public string? PmtargetPk { get; set; }
        public string? EmpAnnualOrderDateTargetPk { get; set; }
        public decimal? Quantity { get; set; }
        public int? Blamount { get; set; }
        public int? Soamount { get; set; }
        public decimal? Soprofit { get; set; }
        public DateTime InsertTime { get; set; }
        public Guid? Id { get; set; }
    }
}
