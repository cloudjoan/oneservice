using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneContractMain
    {
        public string CContractId { get; set; } = null!;
        public string? CSoNo { get; set; }
        public string? CSoSales { get; set; }
        public string? CSoSalesName { get; set; }
        public string? CSoSalesAss { get; set; }
        public string? CSoSalesAssname { get; set; }
        public string? CMasales { get; set; }
        public string? CMasalesName { get; set; }
        public string? CCustomerId { get; set; }
        public string? CCustomerName { get; set; }
        public string? CDesc { get; set; }
        public DateTime? CStartDate { get; set; }
        public DateTime? CEndDate { get; set; }
        public string? CMacycle { get; set; }
        public string? CManotes { get; set; }
        public string? CMaaddress { get; set; }
        public string? CSlaresp { get; set; }
        public string? CSlasrv { get; set; }
        public string? CContractNotes { get; set; }
        public string? CContractReport { get; set; }
        public string? CTeamId { get; set; }
        public string? CIsSubContract { get; set; }
        public string? CBillCycle { get; set; }
        public string? CBillNotes { get; set; }
        public string? CInvalidReason { get; set; }
        public string? CContactName { get; set; }
        public string? CContactEmail { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
