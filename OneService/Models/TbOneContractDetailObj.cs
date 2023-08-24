using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneContractDetailObj
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
        /// 主機名稱
        /// </summary>
        public string? CHostName { get; set; }
        /// <summary>
        /// 序號
        /// </summary>
        public string? CSerialId { get; set; }
        /// <summary>
        /// PID
        /// </summary>
        public string? CPid { get; set; }
        /// <summary>
        /// 廠牌
        /// </summary>
        public string? CBrands { get; set; }
        /// <summary>
        /// 產品模組
        /// </summary>
        public string? CModel { get; set; }
        /// <summary>
        /// Location
        /// </summary>
        public string? CLocation { get; set; }
        /// <summary>
        /// 地點
        /// </summary>
        public string? CAddress { get; set; }
        /// <summary>
        /// 區域
        /// </summary>
        public string? CArea { get; set; }
        /// <summary>
        /// SLA回應條件
        /// </summary>
        public string? CSlaresp { get; set; }
        /// <summary>
        /// SLA服務條件
        /// </summary>
        public string? CSlasrv { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string? CNotes { get; set; }
        /// <summary>
        /// 下包文件編號
        /// </summary>
        public string? CSubContractId { get; set; }
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
