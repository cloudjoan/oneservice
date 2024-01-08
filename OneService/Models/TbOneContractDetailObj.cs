using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneContractDetailObj
    {
        public int CId { get; set; }
        public string CContractId { get; set; } = null!;
        public string? CHostName { get; set; }
        public string? CSerialId { get; set; }
        public string? CPid { get; set; }
        public string? CBrands { get; set; }
        public string? CModel { get; set; }
        public string? CLocation { get; set; }
        public string? CAddress { get; set; }
        public string? CArea { get; set; }
        public string? CSlaresp { get; set; }
        public string? CSlasrv { get; set; }
        public string? CNotes { get; set; }
        public string? CSubContractId { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
