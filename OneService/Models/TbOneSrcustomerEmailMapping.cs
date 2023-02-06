using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrcustomerEmailMapping
    {
        public int CId { get; set; }
        public string? CCustomerId { get; set; }
        public string? CCustomerName { get; set; }
        public string? CTeamId { get; set; }
        public string? CEmailId { get; set; }
        public string? CContactName { get; set; }
        public string? CContactPhone { get; set; }
        public string? CContactEmail { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
