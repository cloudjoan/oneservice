using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbServicesAppInstall
    {
        public int Id { get; set; }
        public string? Srid { get; set; }
        public string? Account { get; set; }
        public string? ErpId { get; set; }
        public string? EmpName { get; set; }
        public string? InstallDate { get; set; }
        public string? ExpectedDate { get; set; }
        public decimal? TotalQuantity { get; set; }
        public decimal? InstallQuantity { get; set; }
        public string? InsertTime { get; set; }
        public string? UpdateAccount { get; set; }
        public string? UpdateEmpName { get; set; }
        public string? UpdateTime { get; set; }
    }
}
