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
        public string? CAttachement { get; set; }
        public string? CAttachementStockNo { get; set; }
        public string? CStatus { get; set; }
        public string? CMaserviceType { get; set; }
        public string? CDelayReason { get; set; }
        public DateTime? CScheduleDate { get; set; }
        public string? CSrtypeOne { get; set; }
        public string? CSrtypeSec { get; set; }
        public string? CSrtypeThr { get; set; }
        public string? CSrpathWay { get; set; }
        public string? CSrprocessWay { get; set; }
        public string? CSrrepairLevel { get; set; }
        public string? CIsSecondFix { get; set; }
        public string? CIsAppclose { get; set; }
        public string? CIsInternalWork { get; set; }
        public string? CRepairName { get; set; }
        public string? CRepairAddress { get; set; }
        public string? CRepairPhone { get; set; }
        public string? CRepairMobile { get; set; }
        public string? CRepairEmail { get; set; }
        public string? CTeamId { get; set; }
        public string? CMainEngineerId { get; set; }
        public string? CMainEngineerName { get; set; }
        public string? CAssEngineerId { get; set; }
        public string? CTechTeamId { get; set; }
        public string? CTechManagerId { get; set; }
        public string? CSqpersonId { get; set; }
        public string? CSqpersonName { get; set; }
        public string? CSalesId { get; set; }
        public string? CSalesName { get; set; }
        public string? CSecretaryId { get; set; }
        public string? CSecretaryName { get; set; }
        public string? CSalesNo { get; set; }
        public string? CShipmentNo { get; set; }
        public string? CContractId { get; set; }
        public int? CCountIn { get; set; }
        public int? CCountOut { get; set; }
        public string? CFaultGroup { get; set; }
        public string? CFaultGroupNote1 { get; set; }
        public string? CFaultGroupNote2 { get; set; }
        public string? CFaultGroupNote3 { get; set; }
        public string? CFaultGroupNote4 { get; set; }
        public string? CFaultGroupNoteOther { get; set; }
        public string? CFaultState { get; set; }
        public string? CFaultStateNote1 { get; set; }
        public string? CFaultStateNote2 { get; set; }
        public string? CFaultStateNoteOther { get; set; }
        public string? CFaultSpec { get; set; }
        public string? CFaultSpecNote1 { get; set; }
        public string? CFaultSpecNote2 { get; set; }
        public string? CFaultSpecNoteOther { get; set; }
        public string? CPerCallSlaresp { get; set; }
        public string? CPerCallSlasrv { get; set; }
        public string? CRemark { get; set; }
        public string? CCustomerUnitType { get; set; }
        public Guid? CSystemGuid { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
