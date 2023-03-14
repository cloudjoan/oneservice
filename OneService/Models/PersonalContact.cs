using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class PersonalContact
    {
        public Guid ContactId { get; set; }
        public string Kna1Kunnr { get; set; } = null!;
        public string? Kna1Name1 { get; set; }
        public string Knb1Bukrs { get; set; } = null!;
        public string ContactName { get; set; } = null!;
        public string? ContactCity { get; set; }
        public string? ContactAddress { get; set; }
        public string ContactEmail { get; set; } = null!;
        public string? ContactPhone { get; set; }
        public string? ContactMobile { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
