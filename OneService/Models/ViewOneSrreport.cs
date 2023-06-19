using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class ViewOneSrreport
    {
        public string? CSrid { get; set; }
        public string? CDesc { get; set; }
        public string? CNotes { get; set; }
        public string? CSrtype { get; set; }
        public string? CSrtypeNote { get; set; }
        public string? CStatusNote { get; set; }
        public string? CStatus { get; set; }
        public string? CCustomerId { get; set; }
        public string? CCustomerName { get; set; }
        public string? CRepairName { get; set; }
        public string? CRepairAddress { get; set; }
        public string? CRepairPhone { get; set; }
        public string? CRepairMobile { get; set; }
        public string? CRepairEmail { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CIsSecondFix { get; set; }
        public string? CIsAppclose { get; set; }
        public string? CTeamId { get; set; }
        public string CSqpersonName { get; set; } = null!;
        public string CDelayReason { get; set; } = null!;
        public string CSrprocessWay { get; set; } = null!;
        public string CMaserviceType { get; set; } = null!;
        public string CSrtypeOne { get; set; } = null!;
        public string CSrtypeOneNote { get; set; } = null!;
        public string CSrtypeSec { get; set; } = null!;
        public string CSrtypeSecNote { get; set; } = null!;
        public string CSrtypeThr { get; set; } = null!;
        public string CSrtypeThrNote { get; set; } = null!;
        public string Pid { get; set; } = null!;
        public string Pn { get; set; } = null!;
        public string CSerialId { get; set; } = null!;
        public DateTime CReceiveTime { get; set; }
        public DateTime? CStartTime { get; set; }
        public DateTime? CArriveTime { get; set; }
        public DateTime? CFinishTime { get; set; }
        public decimal CWorkHours { get; set; }
        public string CEngineerId { get; set; } = null!;
        public string CEngineerName { get; set; } = null!;
        public string CDescR { get; set; } = null!;
        public string CSrreport { get; set; } = null!;
        public string CWtyid { get; set; } = null!;
        public string CWtyname { get; set; } = null!;
        public DateTime CWtysdate { get; set; }
        public DateTime CWtyedate { get; set; }
        public string CSlaresp { get; set; } = null!;
        public string CSlasrv { get; set; } = null!;
        public string CContractId { get; set; } = null!;
        public string CMaterialId { get; set; } = null!;
        public string CMaterialName { get; set; } = null!;
        public string CXchp { get; set; } = null!;
        public string COldCt { get; set; } = null!;
        public string CNewCt { get; set; } = null!;
        public string CHpct { get; set; } = null!;
        public string CPersonalDamage { get; set; } = null!;
        public string CNotePr { get; set; } = null!;
        public string CountIn { get; set; } = null!;
        public string CountOut { get; set; } = null!;
        public string So { get; set; } = null!;
        public string Dn { get; set; } = null!;
    }
}
