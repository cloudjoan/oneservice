using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbProPjinfo
    {
        public int Id { get; set; }
        public string? CrmOppNo { get; set; }
        public string? State { get; set; }
        public string? StateUpdateTime { get; set; }
        public string? StartDate { get; set; }
        public string? StartDateUpdateTime { get; set; }
        public string? EndDate { get; set; }
        public string? EndDateUpdateTime { get; set; }
        public string? ExpEndDate { get; set; }
        public string? ExpEndDateUpdateTime { get; set; }
        public string? KickOffDate { get; set; }
        public string? KickOffDateUpdateTime { get; set; }
        public string? Pmacc { get; set; }
        public string? PmaccUpdateTime { get; set; }
        public string? InsertTime { get; set; }
    }
}
