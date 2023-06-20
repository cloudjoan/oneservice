using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneContractMain
    {
        /// <summary>
        /// 文件編號
        /// </summary>
        public string CContractId { get; set; } = null!;
        /// <summary>
        /// 銷售訂單號
        /// </summary>
        public string? CSoNo { get; set; }
        /// <summary>
        /// 業務員ERPID
        /// </summary>
        public string? CSoSales { get; set; }
        /// <summary>
        /// 業務員姓名(中文+ 英文)
        /// </summary>
        public string? CSoSalesName { get; set; }
        /// <summary>
        /// 業務祕書ERPID
        /// </summary>
        public string? CSoSalesAss { get; set; }
        /// <summary>
        /// 業務祕書姓名(中文+ 英文)
        /// </summary>
        public string? CSoSalesAssname { get; set; }
        /// <summary>
        /// 維護業務員ERPID
        /// </summary>
        public string? CMasales { get; set; }
        /// <summary>
        /// 維護業務員姓名(中文+ 英文)
        /// </summary>
        public string? CMasalesName { get; set; }
        /// <summary>
        /// 客戶代號
        /// </summary>
        public string? CCustomerId { get; set; }
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string? CCustomerName { get; set; }
        /// <summary>
        /// 訂單說明
        /// </summary>
        public string? CDesc { get; set; }
        /// <summary>
        /// 維護開始日期
        /// </summary>
        public DateTime? CStartDate { get; set; }
        /// <summary>
        /// 維護結束日期
        /// </summary>
        public DateTime? CEndDate { get; set; }
        /// <summary>
        /// 維護週期
        /// </summary>
        public string? CMacycle { get; set; }
        /// <summary>
        /// 維護備註
        /// </summary>
        public string? CManotes { get; set; }
        /// <summary>
        /// 維護地址
        /// </summary>
        public string? CMaaddress { get; set; }
        /// <summary>
        /// SLA回應條件
        /// </summary>
        public string? CSlaresp { get; set; }
        /// <summary>
        /// SLA服務條件
        /// </summary>
        public string? CSlasrv { get; set; }
        /// <summary>
        /// 合約備註
        /// </summary>
        public string? CContractNotes { get; set; }
        /// <summary>
        /// 合約書URL
        /// </summary>
        public string? CContractReport { get; set; }
        /// <summary>
        /// 服務團隊ID
        /// </summary>
        public string? CTeamId { get; set; }
        /// <summary>
        /// 是否為下包約(Y、空白)
        /// </summary>
        public string? CIsSubContract { get; set; }
        /// <summary>
        /// 請款週期
        /// </summary>
        public string? CBillCycle { get; set; }
        /// <summary>
        /// 請款備註
        /// </summary>
        public string? CBillNotes { get; set; }
        public string? CInvalidReason { get; set; }
        public string? CContactName { get; set; }
        public string? CContactEmail { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
