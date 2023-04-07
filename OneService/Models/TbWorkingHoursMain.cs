using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbWorkingHoursMain
    {
        public int Id { get; set; }
        public string? UserErpId { get; set; }
        public string? UserName { get; set; }
        public string? Whtype { get; set; }
        public string? ActType { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int? Labor { get; set; }
        public string? CrmOppNo { get; set; }
        public string? CrmOppName { get; set; }
        public string? WhDescript { get; set; }
        public int? PrId { get; set; }
        public string? InsertTime { get; set; }
        public string? UpdateTime { get; set; }
        public int? Disabled { get; set; }
        public string? ModifyUser { get; set; }
    }
}
