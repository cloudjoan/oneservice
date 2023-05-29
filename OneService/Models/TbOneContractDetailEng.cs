using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneContractDetailEng
    {
        /// <summary>
        /// 系統ID
        /// </summary>
        public int CId { get; set; }
        /// <summary>
        /// 文件編號
        /// </summary>
        public string CContractId { get; set; } = null!;
        /// <summary>
        /// 工程師ERPID
        /// </summary>
        public string? CEngineerId { get; set; }
        /// <summary>
        /// 工程師姓名(中文+ 英文)
        /// </summary>
        public string? CEngineerName { get; set; }
        /// <summary>
        /// 是否為主要工程師(Y、空白)
        /// </summary>
        public string? CIsMainEngineer { get; set; }
        /// <summary>
        /// 客戶聯絡人門市代號
        /// </summary>
        public Guid? CContactStoreId { get; set; }
        /// <summary>
        /// 客戶聯絡人門市名稱
        /// </summary>
        public string? CContactStoreName { get; set; }
        /// <summary>
        /// 是否停用(0.啟用 1.停用)
        /// </summary>
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
