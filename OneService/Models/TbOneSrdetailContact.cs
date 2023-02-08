using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrdetailContact
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string? CContactName { get; set; }
        public string? CContactAddress { get; set; }
        public string? CContactPhone { get; set; }
        public string? CContactMobile { get; set; }
        public string? CContactEmail { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
