using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TblFormGuaranteePop
    {
        public string CFormNo { get; set; } = null!;
        public string? CApplyName { get; set; }
        public string? CApplyTel { get; set; }
        public string? CApplyEmail { get; set; }
        public string? CCustName { get; set; }
        public string? CCustTel { get; set; }
        public string? CCustEmail { get; set; }
        public string? CCustComp { get; set; }
        public string? CCustAddr { get; set; }
        public string? CProdSn { get; set; }
        public string? CProdName { get; set; }
        public string? CCounts { get; set; }
        public string? CReceiptDate { get; set; }
        public string? CReceiptNo { get; set; }
        public string? COrgWarrantyDate { get; set; }
        public string? CWarrantyDate { get; set; }
        public string? CPurpose { get; set; }
    }
}
