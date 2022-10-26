using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbProTask
    {
        public int Id { get; set; }
        public string? CrmOppNo { get; set; }
        public string? Milestone { get; set; }
        public int? Stair1No { get; set; }
        public int? Stair2No { get; set; }
        public int? Stair3No { get; set; }
        public string? Description { get; set; }
        public decimal? ExpWorkHours { get; set; }
        public decimal? WorkHours { get; set; }
        public string? ImplementedBy { get; set; }
        public string? TaskState { get; set; }
        public string? ExpStartDate { get; set; }
        public string? ExpEndDate { get; set; }
        public string? ExpDays { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? ActualDays { get; set; }
        public string? PreWork { get; set; }
        public string? ProgressPercentage { get; set; }
        public string? Note { get; set; }
        public string? IncludeHoliday { get; set; }
        public string? CrErpId { get; set; }
        public string? CrAccount { get; set; }
        public string? CrName { get; set; }
        public string? CrEmail { get; set; }
        public string? CrCompCode { get; set; }
        public string? CrDeptId { get; set; }
        public string? CrDeptName { get; set; }
        public int? Disabled { get; set; }
        public string? InsertTime { get; set; }
        public string? UpdateTime { get; set; }
    }
}
