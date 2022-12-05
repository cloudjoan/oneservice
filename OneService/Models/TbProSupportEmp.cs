using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbProSupportEmp
    {
        public int Id { get; set; }
        public string? CrmOppNo { get; set; }
        public string? PartnerId { get; set; }
        public string? SupErpId { get; set; }
        public string? SupName { get; set; }
        public string? SupEmail { get; set; }
        public string? SupExt { get; set; }
        public string? SupMobile { get; set; }
        public string? SupCompCode { get; set; }
        public string? SupDeptId { get; set; }
        public string? SupDeptName { get; set; }
        public string? CrErpId { get; set; }
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
