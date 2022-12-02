using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class ViewWorkingHour
    {
        public int Id { get; set; }
        public string? SrId { get; set; }
        public string? WhDescript { get; set; }
        public string? Whtype { get; set; }
        public string? ActType { get; set; }
        public string? UserErpId { get; set; }
        public string? UserName { get; set; }
        public string? FinishTime { get; set; }
        public int? Labor { get; set; }
        public string SourceFrom { get; set; } = null!;
    }
}
