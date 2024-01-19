using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbLuckydrawPrize
    {
        public int PrizeId { get; set; }
        public int? DrawId { get; set; }
        public int SortNo { get; set; }
        public string? PrizeName { get; set; }
        public string? PrizePic { get; set; }
        public int? PrizeAmount { get; set; }
        public bool OverAyearMark { get; set; }
        public int? DrawAmount { get; set; }
        public int? PrizePrice { get; set; }
        public string? InsertUser { get; set; }
        public string? InsertTime { get; set; }
        public string? ModifyUser { get; set; }
        public string? ModifyTime { get; set; }
        public bool DisabledMark { get; set; }
        public string? PrizeMemo { get; set; }
    }
}
