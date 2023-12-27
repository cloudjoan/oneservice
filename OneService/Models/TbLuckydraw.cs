using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbLuckydraw
    {
        public int DrawId { get; set; }
        public string? DrawYear { get; set; }
        public string? DrawName { get; set; }
        public string? InsertUser { get; set; }
        public string? InsertTime { get; set; }
        public string? ModifyUser { get; set; }
        public string? ModifyTime { get; set; }
        public bool DisabledMark { get; set; }
    }
}
