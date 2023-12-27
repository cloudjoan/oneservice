using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbLuckydrawPrizewinning
    {
        public int WinningId { get; set; }
        public int? PrizeId { get; set; }
        public string? UserName { get; set; }
        public string? UserErpid { get; set; }
        public string? InsertUser { get; set; }
        public string? InsertTime { get; set; }
        public string? ModifyUser { get; set; }
        public string? ModifyTime { get; set; }
        public bool DisabledMark { get; set; }
    }
}
