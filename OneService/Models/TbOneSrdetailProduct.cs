using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrdetailProduct
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string CSerialId { get; set; } = null!;
        public string? CMaterialId { get; set; }
        public string? CMaterialName { get; set; }
        public string? CProductNumber { get; set; }
        public string? CInstallId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
