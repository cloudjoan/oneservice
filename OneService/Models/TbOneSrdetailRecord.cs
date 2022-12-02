using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrdetailRecord
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string CEngineerId { get; set; } = null!;
        public string? CEngineerName { get; set; }
        public DateTime? CReceiveTime { get; set; }
        public DateTime? CStartTime { get; set; }
        public DateTime? CArriveTime { get; set; }
        public DateTime? CFinishTime { get; set; }
        public decimal? CWorkHours { get; set; }
        public string? CDesc { get; set; }
        public string? CSrreport { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
