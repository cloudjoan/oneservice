using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneOperationParameter
    {
        public Guid CId { get; set; }
        public string? CModuleId { get; set; }
        public string? COperationId { get; set; }
        public string? COperationName { get; set; }
        public string? COperationUrl { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
