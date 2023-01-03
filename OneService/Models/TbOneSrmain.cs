using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrmain
    {
        public string CSrid { get; set; } = null!;
        public string? CCustomerId { get; set; }
        public string? CCustomerName { get; set; }
        public string? CDesc { get; set; }
        public string? CNotes { get; set; }
        public string? CAttatchement { get; set; }
        public string? CStatus { get; set; }
        public string? CMaunit { get; set; }
        public string? CMaserviceType { get; set; }
        public string? CDelayReason { get; set; }
        public string? CSrtypeOne { get; set; }
        public string? CSrtypeSec { get; set; }
        public string? CSrtypeThr { get; set; }
        public string? CSrpathWay { get; set; }
        public string? CSrprocessWay { get; set; }
        public string? CIsSecondFix { get; set; }
        public string? CRepairName { get; set; }
        public string? CContacterName { get; set; }
        public string? CContactAddress { get; set; }
        public string? CContactPhone { get; set; }
        public string? CContactEmail { get; set; }
        public string? CTeamId { get; set; }
        public string? CMainEngineerId { get; set; }
        public string? CMainEngineerName { get; set; }
        public string? CAssEngineerId { get; set; }
        public string? CSqpersonId { get; set; }
        public string? CSqpersonName { get; set; }
        public string? CSalesId { get; set; }
        public string? CSalesName { get; set; }
        public Guid? CSystemGuid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
