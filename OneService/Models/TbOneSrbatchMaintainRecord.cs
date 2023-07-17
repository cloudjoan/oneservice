using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrbatchMaintainRecord
    {
        public int CId { get; set; }
        public string CContractId { get; set; } = null!;
        public string? CBukrs { get; set; }
        public string? CCustomerId { get; set; }
        public string? CCustomerName { get; set; }
        public string? CContactStoreName { get; set; }
        public string? CContactName { get; set; }
        public string? CContactAddress { get; set; }
        public string? CContactPhone { get; set; }
        public string? CContactMobile { get; set; }
        public string? CContactEmail { get; set; }
        public string? CMainEngineerId { get; set; }
        public string? CMainEngineerName { get; set; }
        public string? CMacycle { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
