using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class Department
    {
        /// <summary>
        /// 部門代碼
        /// </summary>
        public string Id { get; set; } = null!;
        /// <summary>
        /// 上層部門代碼
        /// </summary>
        public string? ParentId { get; set; }
        public int? DisplayOrder { get; set; }
        public string Name { get; set; } = null!;
        /// <summary>
        /// 部門名稱(中文)
        /// </summary>
        public string? Name2 { get; set; }
        public string? FullName { get; set; }
        public string? FullName2 { get; set; }
        public string? LocationId { get; set; }
        public bool IsCostCenter { get; set; }
        public bool IsVirtual { get; set; }
        public string? Email { get; set; }
        /// <summary>
        /// 主管員工編號
        /// </summary>
        public string? ManagerId { get; set; }
        public string? DeptCode1 { get; set; }
        public string? DeptCode2 { get; set; }
        public string? DeptCode3 { get; set; }
        public string? DeptCode4 { get; set; }
        public string? DeptCode5 { get; set; }
        public string? PrintNum { get; set; }
        /// <summary>
        /// 部門階層
        /// </summary>
        public int Level { get; set; }
        public bool? VisitStore { get; set; }
        public string? VisitStoreUnit { get; set; }
        public int Status { get; set; }
        /// <summary>
        /// JDE部門代碼
        /// </summary>
        public string? JdeDeptNo { get; set; }
        /// <summary>
        /// JDE部門名稱
        /// </summary>
        public string? JdeDeptNm { get; set; }
        public string CrUser { get; set; } = null!;
        public DateTime? CrDate { get; set; }
        public string? Userstamp { get; set; }
        public DateTime? Datestamp { get; set; }
        /// <summary>
        /// BU
        /// </summary>
        public bool IsBusinessUnit { get; set; }
        public string? ProfitCenterId { get; set; }
        public string? CostCenterId { get; set; }
        public string? CompCde { get; set; }
    }
}
