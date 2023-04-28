using System;
using System.Collections.Generic;

namespace OneService.Models
{
    /// <summary>
    /// 服務請求總表
    /// </summary>
    public partial class TbSrReport
    {
        public int Id { get; set; }
        /// <summary>
        /// 服務請求id
        /// </summary>
        public string? ObjectId { get; set; }
        /// <summary>
        /// 說明
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 類型
        /// </summary>
        public string? ProcessType { get; set; }
        /// <summary>
        /// 類型說明
        /// </summary>
        public string? ProcessTypeText { get; set; }
        /// <summary>
        /// 狀態說明
        /// </summary>
        public string? StatusText { get; set; }
        /// <summary>
        /// 狀態
        /// </summary>
        public string? Status { get; set; }
        /// <summary>
        /// 客戶ID
        /// </summary>
        public string? Customerid { get; set; }
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string? Customer { get; set; }
        /// <summary>
        /// 聯絡人ID
        /// </summary>
        public string? Contactid { get; set; }
        /// <summary>
        /// 聯絡人
        /// </summary>
        public string? Contact { get; set; }
        public string? Shop { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// 電話
        /// </summary>
        public string? Tel { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Oa { get; set; }
        public string? Create { get; set; }
        public string? Pid { get; set; }
        public string? Pn { get; set; }
        public string? Sn { get; set; }
        /// <summary>
        /// SOLUTION
        /// </summary>
        public string? Solution { get; set; }
        public string? Software { get; set; }
        public string? Hardware { get; set; }
        public string? Other { get; set; }
        public string? Reset { get; set; }
        public string? Restore { get; set; }
        public string? Replace { get; set; }
        /// <summary>
        /// 服務報告ID
        /// </summary>
        public string? Csrid { get; set; }
        /// <summary>
        /// 服務報告書連結
        /// </summary>
        public string? Csrlink { get; set; }
        public string? Depart { get; set; }
        /// <summary>
        /// 到場
        /// </summary>
        public string? Arrive { get; set; }
        /// <summary>
        /// 服務結束
        /// </summary>
        public string? Finish { get; set; }
        public string? Labor { get; set; }
        /// <summary>
        /// 工程師ID
        /// </summary>
        public string? Engineerid { get; set; }
        /// <summary>
        /// 工程師姓名
        /// </summary>
        public string? Engineer { get; set; }
        /// <summary>
        /// 維護服務單位
        /// </summary>
        public string? Unit { get; set; }
        /// <summary>
        /// 維護服務種類
        /// </summary>
        public string? Srvkind { get; set; }
        public string? Wtyid { get; set; }
        /// <summary>
        /// 保固描述
        /// </summary>
        public string? Wtydesc { get; set; }
        /// <summary>
        /// 保固開始
        /// </summary>
        public string? WtyStart { get; set; }
        /// <summary>
        /// 保固結束
        /// </summary>
        public string? WtyEnd { get; set; }
        /// <summary>
        /// 回應條件
        /// </summary>
        public string? Slaresp { get; set; }
        /// <summary>
        /// 服務條件
        /// </summary>
        public string? Slasrv { get; set; }
        /// <summary>
        /// 報修類別
        /// </summary>
        public string? Rkind1 { get; set; }
        /// <summary>
        /// 報修大類
        /// </summary>
        public string? Rkind2 { get; set; }
        /// <summary>
        /// 報修代碼
        /// </summary>
        public string? Rkind3 { get; set; }
        public string? Sq { get; set; }
        /// <summary>
        /// 業務例外
        /// </summary>
        public string? Sales { get; set; }
        /// <summary>
        /// 處理方式
        /// </summary>
        public string? Dealway { get; set; }
        /// <summary>
        /// 延遲原因
        /// </summary>
        public string? Reason { get; set; }
        /// <summary>
        /// 計數器(IN)
        /// </summary>
        public string? Countin { get; set; }
        /// <summary>
        /// 計數器(OUT)
        /// </summary>
        public string? Countout { get; set; }
        /// <summary>
        /// 二修
        /// </summary>
        public string? Refix { get; set; }
        /// <summary>
        /// 銷售單號
        /// </summary>
        public string? So { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string? Dn { get; set; }
        /// <summary>
        /// 合約編號
        /// </summary>
        public string? Contract { get; set; }
        public string? Problem { get; set; }
        public string? Hpxc { get; set; }
        public string? Oldct { get; set; }
        public string? Newct { get; set; }
        public string? Hpct { get; set; }
        public string? Backupsn { get; set; }
        public string? Changepart { get; set; }
        /// <summary>
        /// 服務團隊
        /// </summary>
        public string? Srvteam { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public string? InsertTime { get; set; }
    }
}
