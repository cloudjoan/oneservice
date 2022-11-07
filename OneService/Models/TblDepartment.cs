using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TblDepartment
    {
        public Guid CDepartmentId { get; set; }
        public string CDepartmentNo { get; set; } = null!;
        public int? CDepartmentDisplayOrder { get; set; }
        public string? CDepartmentName { get; set; }
        public string? CDepartmentEname { get; set; }
        public Guid? CDepartmentManager { get; set; }
        public Guid? CDepartmentParentId { get; set; }
        public byte CDepartmentLevel { get; set; }
        public bool? CDepartmentIsEnable { get; set; }
        public string? CDepartmentLocationId { get; set; }
        public bool? CDepartmentIsCostCenter { get; set; }
        public bool? CDepartmentIsVirtual { get; set; }
        public string? CDepartmentStatus { get; set; }
        public bool? CDepartmentIsBusinessUnit { get; set; }
        public DateTime? CDepartmentCreateDate { get; set; }
        public Guid? CDepartmentCreateUser { get; set; }
        public DateTime? CDepartmentModifyDate { get; set; }
        public Guid? CDepartmentModifyUser { get; set; }
        public string? Pk { get; set; }
        public int? CDisabledUpdate { get; set; }
        public Guid? CCrossDeptId { get; set; }
    }
}
