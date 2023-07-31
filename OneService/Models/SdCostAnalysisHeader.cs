using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class SdCostAnalysisHeader
    {
        public string FormNo { get; set; } = null!;
        public string Year { get; set; } = null!;
        public string? CrmOppNo { get; set; }
        public string? OppPhasePercent { get; set; }
        public string? OppType { get; set; }
        public string? CustomerId { get; set; }
        public string? DeptId { get; set; }
        public string? ProfitCenterId { get; set; }
        public string? Customer { get; set; }
        public string? CustomerGroup { get; set; }
        public string? Paindustry { get; set; }
        public string? Pa { get; set; }
        public string? Patype { get; set; }
        public string? Company { get; set; }
        public string? EmployeeNo { get; set; }
        public string? EmployeeErpid { get; set; }
        public string? FullDepartmentName { get; set; }
        public string? Bg { get; set; }
        public string? Employee { get; set; }
        public DateTime? ApplyDate { get; set; }
        public decimal? TotalPriceAddTax { get; set; }
        public decimal? TotalPriceAll { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PreProfit { get; set; }
        public decimal? EstimateProfit { get; set; }
        public float? EstimateProfitIncluInterCost { get; set; }
        public string? EstimateProfitInInter { get; set; }
        public float? TotalCost { get; set; }
        public float? PreProfitPercentage { get; set; }
        public float? EstimateProfitPercentage { get; set; }
        public float? TechCharge { get; set; }
        public float? PrepareProCharge { get; set; }
        public float? PrepareRate { get; set; }
        public float? CostSpare { get; set; }
        public float? TravelExpenses { get; set; }
        public float? ForeignTravelExpenses { get; set; }
        public float? PrepareFinace { get; set; }
        public float? Internal15Cost { get; set; }
        public string? StartDate { get; set; }
        public string? DealDate { get; set; }
        public string? EstimateDate { get; set; }
        public string? InvoiceDate { get; set; }
        public string? InvoiceYear { get; set; }
        public string? CostType { get; set; }
        public string? InternalNo { get; set; }
        public string? Project { get; set; }
        public int? Sn { get; set; }
        public string? ProjectType { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public DateTime? InsertTime { get; set; }
        public string? InsertUser { get; set; }
        public string? ContractType { get; set; }
        public string? ProductSales { get; set; }
        public float? TotalPriceWithout15 { get; set; }
        public string? Installment { get; set; }
        public float? PreorderAmount { get; set; }
        public float? PreorderProfit { get; set; }
    }
}
