using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneContractDetailSub
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
        /// 下包文件編號
        /// </summary>
        public string CSubContractId { get; set; } = null!;
        /// <summary>
        /// 下包商代號
        /// </summary>
        public string? CSubSupplierId { get; set; }
        /// <summary>
        /// 下包商名稱
        /// </summary>
        public string? CSubSupplierName { get; set; }
        /// <summary>
        /// 下包備註
        /// </summary>
        public string? CSubNotes { get; set; }
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
