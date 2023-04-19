using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrdetailMaterialInfo
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string? CMaterialId { get; set; }
        public string? CMaterialName { get; set; }
        public int? CQuantity { get; set; }
        public string? CBasicContent { get; set; }
        public string? CMfpnumber { get; set; }
        public string? CBrand { get; set; }
        public string? CProductHierarchy { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
