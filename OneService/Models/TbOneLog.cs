using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneLog
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string? EventName { get; set; }
        public string? Log { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
    }
}
