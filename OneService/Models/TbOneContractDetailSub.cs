using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneContractDetailSub
    {
        public int CId { get; set; }
        public string CContractId { get; set; } = null!;
        public string CSubContractId { get; set; } = null!;
        public string? CSubSupplierId { get; set; }
        public string? CSubSupplierName { get; set; }
        public string? CSubNotes { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
