using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class SdOpphead
    {
        /// <summary>
        /// 商機編號
        /// </summary>
        public string OppNo { get; set; } = null!;
        /// <summary>
        /// 員工編號
        /// </summary>
        public string? EmployeeId { get; set; }
        public string? DeptId { get; set; }
        public string? ProfitCenter { get; set; }
        public string? CompId { get; set; }
        /// <summary>
        /// 商機描述
        /// </summary>
        public string? OppDesc { get; set; }
        /// <summary>
        /// 客戶
        /// </summary>
        public string? Customer { get; set; }
        /// <summary>
        /// 商機金額
        /// </summary>
        public int? OppRevenue { get; set; }
        /// <summary>
        /// 起始日起
        /// </summary>
        public DateTime? OppStartDate { get; set; }
        /// <summary>
        /// 結案日期
        /// </summary>
        public DateTime? OppEndDate { get; set; }
        /// <summary>
        /// 出貨日期
        /// </summary>
        public DateTime? OppEstimateDate { get; set; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }
        /// <summary>
        /// 年度月份
        /// </summary>
        public string? YearMonth { get; set; }
        public string? YearMonthOrderDate { get; set; }
        /// <summary>
        /// 商機毛利
        /// </summary>
        public int? OppProfit { get; set; }
        /// <summary>
        /// 商機狀態代碼
        /// </summary>
        public string? OppStatus { get; set; }
        /// <summary>
        /// 商機狀態
        /// </summary>
        public string? OppStatusDesc { get; set; }
        /// <summary>
        /// 商機階段代碼
        /// </summary>
        public string? OppPhase { get; set; }
        /// <summary>
        /// 商機階段
        /// </summary>
        public string? OppPhasePercent { get; set; }
        /// <summary>
        /// 商機階段說明
        /// </summary>
        public string? OppPhaseDesc { get; set; }
        /// <summary>
        /// 商機輸入日期
        /// </summary>
        public DateTime? OppInsertTime { get; set; }
        /// <summary>
        /// 幣別
        /// </summary>
        public string? OppCurrency { get; set; }
        /// <summary>
        /// 銷售循環代碼
        /// </summary>
        public string? OppSalesCycle { get; set; }
        /// <summary>
        /// 銷售循環
        /// </summary>
        public string? OppSalesCycleDesc { get; set; }
        /// <summary>
        /// 客戶編號
        /// </summary>
        public string? CustomerId { get; set; }
        public string? OppOrgResp { get; set; }
        public string? OppOrgShort { get; set; }
        public string? OppOrgChannel { get; set; }
        public string? OppOrgDivision { get; set; }
        public string? OppOrgOffice { get; set; }
        public string? OppOrgGroup { get; set; }
        public string? IndustryApplyFunctionId { get; set; }
        public string? Industry { get; set; }
        /// <summary>
        /// PSO支援狀態
        /// </summary>
        public string? Psostage { get; set; }
        public string? FormNo { get; set; }
        public string? InternalNo { get; set; }
        public string? CancelReason { get; set; }
        public DateTime? InsertTime { get; set; }
        public string? InsertUser { get; set; }
        public int? InternalNoAreadySo { get; set; }
        public string? OppType { get; set; }
        public int? OppInstallment { get; set; }
        public int? NextOppInstallment { get; set; }
        public int? OppInstallmentProfit { get; set; }
        public int? NextOppInstallmentProfit { get; set; }
        public string? Commit { get; set; }
        public string? ContractType { get; set; }
        public string? ProductSales { get; set; }
        public string? InvoiceCommit { get; set; }
    }
}
