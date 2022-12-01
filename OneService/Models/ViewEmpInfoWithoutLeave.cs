using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class ViewEmpInfoWithoutLeave
    {
        public string Id { get; set; } = null!;
        public string? ErpId { get; set; }
        public string Account { get; set; } = null!;
        public string CompName { get; set; } = null!;
        public string? EmpEname { get; set; }
        public string? EmpName { get; set; }
        public string? DeptId { get; set; }
        public string DeptName { get; set; } = null!;
        public string? Constellation { get; set; }
        public DateTime? RegistDate { get; set; }
    }
}
