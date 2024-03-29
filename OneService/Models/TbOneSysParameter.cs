﻿using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSysParameter
    {
        public int CId { get; set; }
        public Guid COperationId { get; set; }
        public string CFunctionId { get; set; } = null!;
        public string CCompanyId { get; set; } = null!;
        public string CNo { get; set; } = null!;
        public string? CValue { get; set; }
        public string? CDescription { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
