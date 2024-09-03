using System;
using System.Collections.Generic;

namespace OneService.Models
{
    /// <summary>
    /// 客戶聯絡人檔
    /// </summary>
    public partial class CustomerContact
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid ContactId { get; set; }
        /// <summary>
        /// 客戶代號
        /// </summary>
        public string Kna1Kunnr { get; set; } = null!;
        /// <summary>
        /// 客戶名稱
        /// </summary>
        public string? Kna1Name1 { get; set; }
        /// <summary>
        /// 公司代碼
        /// </summary>
        public string Knb1Bukrs { get; set; } = null!;
        /// <summary>
        /// 聯絡人類別(0:發票,1:驗收,2:收貨,3:存出保證金經辦,4:客戶建檔聯絡人,5.OneService建立)
        /// </summary>
        public string ContactType { get; set; } = null!;
        /// <summary>
        /// 聯絡人姓名
        /// </summary>
        public string ContactName { get; set; } = null!;
        /// <summary>
        /// 聯絡人縣市
        /// </summary>
        public string? ContactCity { get; set; }
        /// <summary>
        /// 聯絡人地址
        /// </summary>
        public string? ContactAddress { get; set; }
        /// <summary>
        /// 聯絡人Email
        /// </summary>
        public string ContactEmail { get; set; } = null!;
        /// <summary>
        /// 聯絡人電話
        /// </summary>
        public string? ContactPhone { get; set; }
        /// <summary>
        /// 聯絡人手機
        /// </summary>
        public string? ContactMobile { get; set; }
        public Guid? ContactStore { get; set; }
        /// <summary>
        /// 聯絡人部門
        /// </summary>
        public string? ContactDepartment { get; set; }
        /// <summary>
        /// 聯絡人職稱
        /// </summary>
        public string? ContactPosition { get; set; }
        /// <summary>
        /// BPM表單編號
        /// </summary>
        public string BpmNo { get; set; } = null!;
        /// <summary>
        /// 更新者ID
        /// </summary>
        public Guid? ModifiedUserId { get; set; }
        /// <summary>
        /// 更新者姓名
        /// </summary>
        public string? ModifiedUserName { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        public int? Disabled { get; set; }
        /// <summary>
        /// 是否為對帳窗口
        /// </summary>
        public byte? IsMain { get; set; }
        /// <summary>
        /// 對帳窗口更新者ID
        /// </summary>
        public Guid? MainModifiedUserId { get; set; }
        /// <summary>
        /// 對帳窗口更新者姓名
        /// </summary>
        public string? MainModifiedUserName { get; set; }
        /// <summary>
        /// 對帳窗口更新日期
        /// </summary>
        public DateTime? MainModifiedDate { get; set; }
    }
}
