using System;
using System.Collections.Generic;

namespace OneService.Models
{
    /// <summary>
    /// 商機表頭檔
    /// </summary>
    public partial class TbCrmOppHead
    {
        public decimal Id { get; set; }
        /// <summary>
        /// 商機號碼
        /// </summary>
        public string? CrmOppNo { get; set; }
        /// <summary>
        /// 負責員工員編
        /// </summary>
        public string? CreateAccount { get; set; }
        /// <summary>
        /// 商機標題
        /// </summary>
        public string? OppDescription { get; set; }
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string? CompName { get; set; }
        /// <summary>
        /// 預計營收
        /// </summary>
        public string? ExpRevenue { get; set; }
        /// <summary>
        /// 開始日期
        /// </summary>
        public string? StartDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        public string? ExpEndDate { get; set; }
        /// <summary>
        /// 預計結束日期
        /// </summary>
        public string? EstimateDate { get; set; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public string? InvoiceDate { get; set; }
        /// <summary>
        /// 預計毛利
        /// </summary>
        public string? ExpGrossprofit { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public string? Status { get; set; }
        /// <summary>
        /// 階段
        /// </summary>
        public string? CurrPhase { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public string? InsertTime { get; set; }
        /// <summary>
        /// 幣別
        /// </summary>
        public string? Currency { get; set; }
        /// <summary>
        /// 銷售類型
        /// </summary>
        public string? SalesCycle { get; set; }
        /// <summary>
        /// 客戶ID
        /// </summary>
        public string? CustomerId { get; set; }
        public string? OrderType { get; set; }
        public string? OrgResp { get; set; }
        public string? OrgShort { get; set; }
        public string? DisChannel { get; set; }
        public string? Division { get; set; }
        public string? SalesOffice { get; set; }
        public string? SalesGroup { get; set; }
        public string? IndustryApply { get; set; }
        public string? ProStage { get; set; }
        public string? CancelReason { get; set; }
        public string? ChangeTime { get; set; }
        public string? ContractType { get; set; }
    }
}
