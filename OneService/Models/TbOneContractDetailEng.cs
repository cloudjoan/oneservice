using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneContractDetailEng
    {
        public int CId { get; set; }
        public string CContractId { get; set; } = null!;
        public string? CEngineerId { get; set; }
        public string? CEngineerName { get; set; }
        public string? CIsMainEngineer { get; set; }
        public Guid? CContactStoreId { get; set; }
        public string? CContactStoreName { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
