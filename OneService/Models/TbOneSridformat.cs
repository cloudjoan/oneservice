using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSridformat
    {
        public int CId { get; set; }
        public string CTitle { get; set; } = null!;
        public string CYear { get; set; } = null!;
        public string CMonth { get; set; } = null!;
        public string CDay { get; set; } = null!;
        public string CNo { get; set; } = null!;
    }
}
