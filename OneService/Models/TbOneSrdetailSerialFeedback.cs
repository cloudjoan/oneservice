using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrdetailSerialFeedback
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string CSerialId { get; set; } = null!;
        public string? CMaterialId { get; set; }
        public string? CMaterialName { get; set; }
        public string? CConfigReport { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
