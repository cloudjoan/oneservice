using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrdetailWarranty
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string CSerialId { get; set; } = null!;
        public string? CWtyid { get; set; }
        public string? CWtyname { get; set; }
        public DateTime? CWtysdate { get; set; }
        public DateTime? CWtyedate { get; set; }
        public string? CSlaresp { get; set; }
        public string? CSlasrv { get; set; }
        public string? CContractId { get; set; }
        public string? CBpmformNo { get; set; }
        public string? CAdvice { get; set; }
        public string? CSubContractId { get; set; }
        public string? CUsed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
