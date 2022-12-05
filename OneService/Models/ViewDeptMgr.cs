using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class ViewDeptMgr
    {
        public string DeptCode { get; set; } = null!;
        public string? DeptName { get; set; }
        public string? ErpId { get; set; }
        public int DeptLevel { get; set; }
        public int Disabled { get; set; }
        public string? CompCode { get; set; }
        public string? Up1DeptId { get; set; }
        public string? Up1DeptName { get; set; }
        public string? Up1DeptMgErpId { get; set; }
        public int? Up1Level { get; set; }
        public int? Up1Status { get; set; }
        public string? Up2DpetId { get; set; }
        public string? Up2DeptName { get; set; }
        public string? Up2DeptMgErpId { get; set; }
        public int? Up2Level { get; set; }
        public int? Up2Status { get; set; }
    }
}
