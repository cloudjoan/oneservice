using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneDocument
    {
        public Guid Id { get; set; }
        public string? RefObjId { get; set; }
        public string? FileOrgName { get; set; }
        public string? FileName { get; set; }
        public string? FileExt { get; set; }
        public string? InsertTime { get; set; }
    }
}
