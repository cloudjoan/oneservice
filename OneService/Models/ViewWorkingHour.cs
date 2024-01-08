using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class ViewWorkingHour
    {
        public int Id { get; set; }
        public string? SrId { get; set; }
        public string? CrmOppName { get; set; }
        public string? WhDescript { get; set; }
        public string? Whtype { get; set; }
        public string WhtypeName { get; set; } = null!;
        public string? ActType { get; set; }
        public string? ActTypeName { get; set; }
        public string? UserErpId { get; set; }
        public string? UserName { get; set; }
        public DateTime? FinishTime { get; set; }
        public decimal? Labor { get; set; }
        public string SourceFrom { get; set; } = null!;
    }
}
