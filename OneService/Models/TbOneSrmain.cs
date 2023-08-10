using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrmain
    {
        /// <summary>
        /// 服務ID
        /// </summary>
        public string CSrid { get; set; } = null!;
        /// <summary>
        /// 客戶代號
        /// </summary>
        public string? CCustomerId { get; set; }
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string? CCustomerName { get; set; }
        /// <summary>
        /// 說明
        /// </summary>
        public string? CDesc { get; set; }
        /// <summary>
        /// 詳細描述
        /// </summary>
        public string? CNotes { get; set; }
        /// <summary>
        /// 檢附文件
        /// </summary>
        public string? CAttachement { get; set; }
        /// <summary>
        /// 備料服務通知單文件
        /// </summary>
        public string? CAttachementStockNo { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public string? CStatus { get; set; }
        /// <summary>
        /// 維護服務種類
        /// </summary>
        public string? CMaserviceType { get; set; }
        /// <summary>
        /// 延遲結案原因
        /// </summary>
        public string? CDelayReason { get; set; }
        /// <summary>
        /// 第一階(大類)
        /// </summary>
        public string? CSrtypeOne { get; set; }
        /// <summary>
        /// 第二階(中類)
        /// </summary>
        public string? CSrtypeSec { get; set; }
        /// <summary>
        /// 第三階(小類)
        /// </summary>
        public string? CSrtypeThr { get; set; }
        /// <summary>
        /// 報修管道
        /// </summary>
        public string? CSrpathWay { get; set; }
        /// <summary>
        /// 處理方式
        /// </summary>
        public string? CSrprocessWay { get; set; }
        public string? CSrrepairLevel { get; set; }
        /// <summary>
        /// 是否為二修
        /// </summary>
        public string? CIsSecondFix { get; set; }
        /// <summary>
        /// 是否為APP結案
        /// </summary>
        public string? CIsAppclose { get; set; }
        public string? CIsInternalWork { get; set; }
        /// <summary>
        /// 客戶報修人姓名
        /// </summary>
        public string? CRepairName { get; set; }
        /// <summary>
        /// 客戶報修人地址
        /// </summary>
        public string? CRepairAddress { get; set; }
        /// <summary>
        /// 客戶報修人電話
        /// </summary>
        public string? CRepairPhone { get; set; }
        /// <summary>
        /// 客戶報修人手機
        /// </summary>
        public string? CRepairMobile { get; set; }
        /// <summary>
        /// 客戶報修人Email
        /// </summary>
        public string? CRepairEmail { get; set; }
        /// <summary>
        /// 服務團隊ID
        /// </summary>
        public string? CTeamId { get; set; }
        /// <summary>
        /// 主要工程師ERPID
        /// </summary>
        public string? CMainEngineerId { get; set; }
        /// <summary>
        /// 主要工程師姓名
        /// </summary>
        public string? CMainEngineerName { get; set; }
        /// <summary>
        /// 協助工程師ERPID
        /// </summary>
        public string? CAssEngineerId { get; set; }
        /// <summary>
        /// 技術主管ERPID
        /// </summary>
        public string? CTechManagerId { get; set; }
        /// <summary>
        /// SQ人員ID
        /// </summary>
        public string? CSqpersonId { get; set; }
        /// <summary>
        /// SQ人員名稱
        /// </summary>
        public string? CSqpersonName { get; set; }
        /// <summary>
        /// 計費業務ERPID
        /// </summary>
        public string? CSalesId { get; set; }
        /// <summary>
        /// 計費業務姓名
        /// </summary>
        public string? CSalesName { get; set; }
        /// <summary>
        /// 業務祕書ERPID
        /// </summary>
        public string? CSecretaryId { get; set; }
        /// <summary>
        /// 業務祕書姓名
        /// </summary>
        public string? CSecretaryName { get; set; }
        /// <summary>
        /// 銷售訂單號
        /// </summary>
        public string? CSalesNo { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string? CShipmentNo { get; set; }
        public string? CContractId { get; set; }
        public int? CCountIn { get; set; }
        public int? CCountOut { get; set; }
        /// <summary>
        /// 客戶故障狀況分類
        /// </summary>
        public string? CFaultGroup { get; set; }
        /// <summary>
        /// 客戶故障狀況分類說明-硬體
        /// </summary>
        public string? CFaultGroupNote1 { get; set; }
        /// <summary>
        /// 客戶故障狀況分類說明-系統
        /// </summary>
        public string? CFaultGroupNote2 { get; set; }
        /// <summary>
        /// 客戶故障狀況分類說明-服務
        /// </summary>
        public string? CFaultGroupNote3 { get; set; }
        /// <summary>
        /// 客戶故障狀況分類說明-網路
        /// </summary>
        public string? CFaultGroupNote4 { get; set; }
        /// <summary>
        /// 客戶故障狀況分類說明-其他
        /// </summary>
        public string? CFaultGroupNoteOther { get; set; }
        /// <summary>
        /// 客戶故障狀況
        /// </summary>
        public string? CFaultState { get; set; }
        /// <summary>
        /// 客戶故障狀況說明-面板燈號
        /// </summary>
        public string? CFaultStateNote1 { get; set; }
        /// <summary>
        /// 客戶故障狀況說明-管理介面(iLO、IMM、iDRAC)
        /// </summary>
        public string? CFaultStateNote2 { get; set; }
        /// <summary>
        /// 客戶故障狀況說明-其他
        /// </summary>
        public string? CFaultStateNoteOther { get; set; }
        /// <summary>
        /// 故障零件規格料號
        /// </summary>
        public string? CFaultSpec { get; set; }
        /// <summary>
        /// 故障零件規格料號-零件規格
        /// </summary>
        public string? CFaultSpecNote1 { get; set; }
        /// <summary>
        /// 故障零件規格料號-零件料號
        /// </summary>
        public string? CFaultSpecNote2 { get; set; }
        /// <summary>
        /// 故障零件規格料號-其他
        /// </summary>
        public string? CFaultSpecNoteOther { get; set; }
        /// <summary>
        /// 系統目前GUID
        /// </summary>
        public Guid? CSystemGuid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
