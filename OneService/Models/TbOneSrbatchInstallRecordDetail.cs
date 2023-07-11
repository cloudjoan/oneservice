using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrbatchInstallRecordDetail
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string? CMaterialId { get; set; }
        public string? CMaterialName { get; set; }
        public int? CQuantity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
    }
}
