using System;
using System.Collections.Generic;

namespace OneService.Models
{
    /// <summary>
    /// 銷售訂單檔
    /// </summary>
    public partial class So
    {
        /// <summary>
        /// 信用控制範圍
        /// </summary>
        public string? Kkber { get; set; }
        /// <summary>
        /// 銷售文件類型
        /// </summary>
        public string Auart { get; set; } = null!;
        /// <summary>
        /// 銷售文件
        /// </summary>
        public string Vbeln { get; set; } = null!;
        /// <summary>
        /// 銷售文件項目
        /// </summary>
        public decimal Posnr { get; set; }
        /// <summary>
        /// 銷售組織
        /// </summary>
        public string? Vkorg { get; set; }
        /// <summary>
        /// 配銷通路
        /// </summary>
        public string? Vtweg { get; set; }
        /// <summary>
        /// 部門
        /// </summary>
        public string? Spart { get; set; }
        /// <summary>
        /// 銷售群組
        /// </summary>
        public string? Vkgrp { get; set; }
        /// <summary>
        /// 銷售據點
        /// </summary>
        public string? Vkbur { get; set; }
        /// <summary>
        /// 買方
        /// </summary>
        public string? Kunnr { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string? Name1 { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string? City1 { get; set; }
        /// <summary>
        /// 郵遞區號
        /// </summary>
        public string? PostCode1 { get; set; }
        /// <summary>
        /// 街道
        /// </summary>
        public string? Street1 { get; set; }
        /// <summary>
        /// 國家碼
        /// </summary>
        public string? Country1 { get; set; }
        /// <summary>
        /// 收貨人
        /// </summary>
        public string? Kunnr1 { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string? Name2 { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string? City2 { get; set; }
        /// <summary>
        /// 郵遞區號
        /// </summary>
        public string? PostCode2 { get; set; }
        /// <summary>
        /// 街道
        /// </summary>
        public string? Street2 { get; set; }
        /// <summary>
        /// 國家碼
        /// </summary>
        public string? Country2 { get; set; }
        /// <summary>
        /// 文件日期
        /// </summary>
        public DateTime? Audat { get; set; }
        /// <summary>
        /// 請求交貨日期
        /// </summary>
        public DateTime? Vdatu { get; set; }
        /// <summary>
        /// 供貨日期
        /// </summary>
        public DateTime? Mbdat { get; set; }
        /// <summary>
        /// 物料
        /// </summary>
        public string? Matnr { get; set; }
        /// <summary>
        /// 說明
        /// </summary>
        public string? Arktx { get; set; }
        /// <summary>
        /// 保固條件
        /// </summary>
        public string? Preservation { get; set; }
        /// <summary>
        /// 基礎計量單位
        /// </summary>
        public string? Meins { get; set; }
        /// <summary>
        /// 訂購數量
        /// </summary>
        public decimal? Kwmeng { get; set; }
        /// <summary>
        /// 交貨數量
        /// </summary>
        public decimal? Lgmng { get; set; }
        /// <summary>
        /// 員工號碼
        /// </summary>
        public decimal? Pernr { get; set; }
        /// <summary>
        /// 姓
        /// </summary>
        public string? Nachn { get; set; }
        /// <summary>
        /// 名
        /// </summary>
        public string? Vorna { get; set; }
        /// <summary>
        /// 建立者
        /// </summary>
        public string? Ernam { get; set; }
        /// <summary>
        /// 更改日期
        /// </summary>
        public DateTime? Aedat { get; set; }
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string Vbeln1 { get; set; } = null!;
        /// <summary>
        /// 項目
        /// </summary>
        public decimal Posnn { get; set; }
        /// <summary>
        /// 工廠
        /// </summary>
        public string? Werks { get; set; }
        /// <summary>
        /// 儲存地點
        /// </summary>
        public string? Lgort { get; set; }
        /// <summary>
        /// 出貨點/收貨點
        /// </summary>
        public string? Vstel { get; set; }
        /// <summary>
        /// 採購單號碼
        /// </summary>
        public string? Bstnk { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime? Erdat { get; set; }
        /// <summary>
        /// 輸入時間
        /// </summary>
        public DateTime? Aedat1 { get; set; }
        public string? Abgru { get; set; }
        public string? BstkdE { get; set; }
        public DateTime? PinvoiceDate { get; set; }
        public string? PaymentTerms { get; set; }
        public decimal? Netpr { get; set; }
        public string? ProfitCenterId { get; set; }
        public string? Status { get; set; }
        public string? Waerk { get; set; }
        public string? OldProfitCenterId { get; set; }
        public decimal? Vvq01 { get; set; }
        public decimal? Vvr01 { get; set; }
        public decimal? Vvc01 { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public string? PurchaseWaers { get; set; }
        public decimal? InvoiceQuantity { get; set; }
        public string? IndustryApply { get; set; }
        public string? ReceiverName2 { get; set; }
        public string? ReceiverName3 { get; set; }
        public string? ReceiverName4 { get; set; }
    }
}
