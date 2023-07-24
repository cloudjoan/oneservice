using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbProPjRecord
    {
        public int Id { get; set; }
        public string? BundleMs { get; set; }
        public string? BundleTask { get; set; }
        public string? CrmOppNo { get; set; }
        public string? ImplementedBy { get; set; }
        public int? ImpPplCount { get; set; }
        public string? Implementers { get; set; }
        public int? ImplementersCount { get; set; }
        public string? Attendees { get; set; }
        public int? AttendeesCount { get; set; }
        public string? Place { get; set; }
        public string? StartDatetime { get; set; }
        public string? EndDatetime { get; set; }
        public decimal? WorkHours { get; set; }
        public decimal? TotalWorkHours { get; set; }
        public string? WithPpl { get; set; }
        public string? WithPplPhone { get; set; }
        public string? Description { get; set; }
        public string? Attachment { get; set; }
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
        public string ImplementersOut { get; set; } = null!;
        public int ImplementersCountOut { get; set; }
    }
}
