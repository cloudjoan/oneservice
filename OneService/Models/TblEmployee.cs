using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TblEmployee
    {
        public Guid CEmployeeId { get; set; }
        public string CEmployeeNo { get; set; } = null!;
        public string CEmployeeCName { get; set; } = null!;
        public string? CEmployeeEname { get; set; }
        public string? CEmployeeTitle { get; set; }
        public Guid CDepartmentId { get; set; }
        public int CEmployeeLevel { get; set; }
        public int? CEmployeePositionCode { get; set; }
        public string? CEmployeePositionCode2 { get; set; }
        public bool CEmployeeAway { get; set; }
        public string? CEmployeeTel { get; set; }
        public string? CEmployeeMobile { get; set; }
        public string? CEmployeeFax { get; set; }
        public string? CEmployeeEmail { get; set; }
        public int CEmployeeNotifyTypeId { get; set; }
        public bool CEmployeePluralism { get; set; }
        public int CEmployeeStatus { get; set; }
        public string CEmployeePid { get; set; } = null!;
        public DateTime? CEmployeeBrithDay { get; set; }
        public string? CEmployeeConstellationId { get; set; }
        public string? CEmployeePhotoPath { get; set; }
        public string CEmployeeSexId { get; set; } = null!;
        public string CEmployeeJobStatusId { get; set; } = null!;
        public DateTime? CEmployeeRegestDate { get; set; }
        public DateTime? CEmployeeCallInDate { get; set; }
        public DateTime? CEmployeeCallOutDate { get; set; }
        public DateTime? CEmployeeReplaceDate { get; set; }
        public DateTime? CEmployeeTakeDay { get; set; }
        public DateTime? CEmployeeLeaveDay { get; set; }
        public string? CEmployeeLeaveReason { get; set; }
        public string? CEmployeeCompanyCode { get; set; }
        public string? CEmployeeAreaCode { get; set; }
        public string? CEmployeeJobTypeId { get; set; }
        public string? CEmployeeTitle1 { get; set; }
        public string? CEmployeeTitle2 { get; set; }
        public string? CEmployeeCapitalPosition1 { get; set; }
        public string? CEmployeeCapitalPosition2 { get; set; }
        public DateTime? CEmployeeCpDate { get; set; }
        public string? CEmployeePosition1 { get; set; }
        public string? CEmployeePosition2 { get; set; }
        public DateTime? CEmployeePositionDate { get; set; }
        public string? CEmployeeCompanyPhone { get; set; }
        public string? CEmployeeCompanyPhoneExt { get; set; }
        public string? CEmployeeNationalityId { get; set; }
        public string? CEmployeeBloodType { get; set; }
        public string? CEmployeeWorkPlace { get; set; }
        public string? CEmployeeErpid { get; set; }
        public string? CEmployeeWorkers { get; set; }
        public string? CEmployeeCenter { get; set; }
        public string? CEmployeeAssignType { get; set; }
        public bool CEmployeeIsEnable { get; set; }
        public string CEmployeeAccount { get; set; } = null!;
        public DateTime CEmployeeCreateDate { get; set; }
        public Guid CEmployeeCreateUser { get; set; }
        public DateTime? CEmployeeModifyDate { get; set; }
        public Guid? CEmployeeModifyUser { get; set; }
        public string? CEmployeeLevel1 { get; set; }
        public string? CEmployeeLevel2 { get; set; }
        public DateTime? CEmployeeLevel1Date { get; set; }
        public DateTime? CEmployeeLevel2Date { get; set; }
        public string? CEmployeeIntroducer { get; set; }
        public bool? CEmployeeIsVegetarian { get; set; }
        public Guid? CEmployeeProfitcenterId { get; set; }
        public Guid? CEmployeeCostCenterId { get; set; }
        public string? Pk { get; set; }
    }
}
