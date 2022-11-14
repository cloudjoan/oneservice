using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class Material
    {
        /// <summary>
        /// 物料號碼
        /// </summary>
        public string MaraMatnr { get; set; } = null!;
        /// <summary>
        /// 工廠
        /// </summary>
        public string MardWerks { get; set; } = null!;
        /// <summary>
        /// 儲存地點
        /// </summary>
        public string MardLgort { get; set; } = null!;
        /// <summary>
        /// 銷售組織
        /// </summary>
        public string MvkeVkorg { get; set; } = null!;
        /// <summary>
        /// 通路
        /// </summary>
        public string MvkeVtweg { get; set; } = null!;
        /// <summary>
        /// 原廠料號欄位-製造商零件號碼
        /// </summary>
        public string? MaraMfrpn { get; set; }
        /// <summary>
        /// 短文(中文-ZF)
        /// </summary>
        public string? MaktTxza1Zf { get; set; }
        /// <summary>
        /// 短文(英文-EN)
        /// </summary>
        public string? MaktTxza1En { get; set; }
        /// <summary>
        /// 保固條件(採購-中文)
        /// </summary>
        public string? MaraStxl1 { get; set; }
        /// <summary>
        /// 保固條件(銷售中文)
        /// </summary>
        public string? MaraStxl2 { get; set; }
        /// <summary>
        /// 保固條件(採購-英文)
        /// </summary>
        public string? MaraStxl3 { get; set; }
        /// <summary>
        /// 保固條件(銷售-英文)
        /// </summary>
        public string? MaraStxl4 { get; set; }
        /// <summary>
        /// 產品階層
        /// </summary>
        public string? MvkeProdh { get; set; }
        /// <summary>
        /// 物料群組
        /// </summary>
        public string? MaraMatkl { get; set; }
        /// <summary>
        /// 儲格
        /// </summary>
        public string? StorageBlock { get; set; }
        /// <summary>
        /// 基本物料內文
        /// </summary>
        public string? BasicContent { get; set; }
    }
}
