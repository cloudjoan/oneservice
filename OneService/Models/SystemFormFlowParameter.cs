using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class SystemFormFlowParameter
    {
        public string CFunctionNo { get; set; } = null!;
        public string CNo { get; set; } = null!;
        public string CType { get; set; } = null!;
        public string CDescription { get; set; } = null!;
        public string CValue { get; set; } = null!;
        public Guid CCreatedUser { get; set; }
        public DateTime CCreatedDate { get; set; }
        public Guid? CModifiedUser { get; set; }
        public DateTime? CModifiedDate { get; set; }
    }
}
