using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbMailContent
    {
        public int Id { get; set; }
        /// <summary>
        /// 郵件類型
        /// </summary>
        public string? MailType { get; set; }
        /// <summary>
        /// 郵件內容
        /// </summary>
        public string? MailContent { get; set; }
    }
}
