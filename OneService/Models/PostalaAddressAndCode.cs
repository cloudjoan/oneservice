using System;
using System.Collections.Generic;

namespace OneService.Models
{
    /// <summary>
    /// 郵遞區域地址檔
    /// </summary>
    public partial class PostalaAddressAndCode
    {
        public string Code { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Township { get; set; } = null!;
        public string Road { get; set; } = null!;
        public string No { get; set; } = null!;
    }
}
