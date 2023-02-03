using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbWhType
    {
        public int Id { get; set; }
        public string? UpTypeCode { get; set; }
        public string? TypeCode { get; set; }
        public string? TypeName { get; set; }
        public int? Sort { get; set; }
    }
}
