using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrrepairType
    {
        public int CId { get; set; }
        public string? CKindKey { get; set; }
        public string? CUpKindKey { get; set; }
        public string? CKindName { get; set; }
        public string? CKindNameEnUs { get; set; }
        public int? CKindLevel { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
