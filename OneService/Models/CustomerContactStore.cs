using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class CustomerContactStore
    {
        public Guid ContactStoreId { get; set; }
        public string ContactStoreName { get; set; } = null!;
        public string Kna1Kunnr { get; set; } = null!;
        public string? Kna1Name1 { get; set; }
        public string Knb1Bukrs { get; set; } = null!;
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
