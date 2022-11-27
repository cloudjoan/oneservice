using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbProMilestone
    {
        public int Id { get; set; }
        public string? CrmOppNo { get; set; }
        public string? MilestoneNo { get; set; }
        public string? Status { get; set; }
        public string? MsDescription { get; set; }
        public string? Tasks { get; set; }
        public string? PaymentPeriod { get; set; }
        public string? EstimatedDate { get; set; }
        public string? WarningDays { get; set; }
        public string? FinishedDate { get; set; }
        public string? DelayReason { get; set; }
        public string? CrErpId { get; set; }
        public string? CrAccount { get; set; }
        public string? CrName { get; set; }
        public string? CrEmail { get; set; }
        public string? CrCompCode { get; set; }
        public string? CrDeptId { get; set; }
        public string? CrDeptName { get; set; }
        public int? Disabled { get; set; }
        public string? IsAlarm { get; set; }
        public string? InsertTime { get; set; }
        public string? UpdateTime { get; set; }
    }
}
