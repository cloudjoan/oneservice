using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbBulletinItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public int BulletinTypeId { get; set; }
        /// <summary>
        /// 公告選項ID
        /// </summary>
        public int BulletinItem { get; set; }
        /// <summary>
        /// 公告選項名稱
        /// </summary>
        public string BulletinItemName { get; set; } = null!;
        /// <summary>
        /// 預設主旨
        /// </summary>
        public string BulletinSubject { get; set; } = null!;
        /// <summary>
        /// 預設內容
        /// </summary>
        public string BulletinContent { get; set; } = null!;
        /// <summary>
        /// 預設模板
        /// </summary>
        public int? DefaultTemplateId { get; set; }
        /// <summary>
        /// 預設是否需簽核
        /// </summary>
        public bool IsApprove { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool IsEnabled { get; set; }
        public string? BulletinRemind { get; set; }
    }
}
