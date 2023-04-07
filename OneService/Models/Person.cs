using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class Person
    {
        public string Id { get; set; } = null!;
        public string Alias { get; set; } = null!;
        /// <summary>
        /// 英文姓名
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 員工姓名
        /// </summary>
        public string? Name2 { get; set; }
        /// <summary>
        /// 所屬部門
        /// </summary>
        public string? DeptId { get; set; }
        public string TitleId { get; set; } = null!;
        public string? TitleName { get; set; }
        public int Level { get; set; }
        public int PositionCode { get; set; }
        public string? AgentId { get; set; }
        /// <summary>
        /// 使用者編號(流水號)
        /// </summary>
        public DateTime? AgentStartTime { get; set; }
        public DateTime? AgentEndTime { get; set; }
        public bool Away { get; set; }
        /// <summary>
        /// E-Mail
        /// </summary>
        public string? Email { get; set; }
        public string? Phone { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        public string? Mobile { get; set; }
        public string? CostCenter { get; set; }
        public int NotifyType { get; set; }
        public bool Pluralism { get; set; }
        public string? PositionCode2 { get; set; }
        public bool? Manager { get; set; }
        public bool? IsDeptManager { get; set; }
        public int Status { get; set; }
        /// <summary>
        /// 員工編號(身份証字號)
        /// </summary>
        public string Pid { get; set; } = null!;
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 星座(CDE_TYPE=CONSTEL)
        /// </summary>
        public string? Constellation { get; set; }
        /// <summary>
        /// 性別(M:男性、F:女性)
        /// </summary>
        public string Sex { get; set; } = null!;
        /// <summary>
        /// 任職現況
        /// </summary>
        public string JobStatus { get; set; } = null!;
        /// <summary>
        /// 到職日期
        /// </summary>
        public DateTime? RegestDate { get; set; }
        /// <summary>
        /// 調入日期
        /// </summary>
        public DateTime? CallInDate { get; set; }
        /// <summary>
        /// 調出日期
        /// </summary>
        public DateTime? CallOutDate { get; set; }
        /// <summary>
        /// 最近一次復職日期
        /// </summary>
        public DateTime? ReplaceDate { get; set; }
        /// <summary>
        /// 離職日期
        /// </summary>
        public DateTime? LeaveDate { get; set; }
        public string? LeaveReason { get; set; }
        /// <summary>
        /// 公司別
        /// </summary>
        public string? CompCde { get; set; }
        /// <summary>
        /// 區域別
        /// </summary>
        public string? AreaCde { get; set; }
        /// <summary>
        /// 職務(CDE_TYPE=JOB)
        /// </summary>
        public string? JobType { get; set; }
        /// <summary>
        /// 職稱(CDE_TYPE=TITLE1)
        /// </summary>
        public string? Title1 { get; set; }
        /// <summary>
        /// 職種(CDE_TYPE=TITLE2)
        /// </summary>
        public string? Title2 { get; set; }
        /// <summary>
        /// 資位一(CDE_TYPE=CP1)
        /// </summary>
        public string? CapitalPosition1 { get; set; }
        /// <summary>
        /// 資位二(CDE_TYPE=CP2)
        /// </summary>
        public string? CapitalPosition2 { get; set; }
        /// <summary>
        /// 資位日期
        /// </summary>
        public DateTime? CpDate { get; set; }
        /// <summary>
        /// 職位一(CDE_TYPE=PT1)
        /// </summary>
        public string? Position1 { get; set; }
        /// <summary>
        /// 職位二(CDE_TYPE=PT2)
        /// </summary>
        public string? Position2 { get; set; }
        /// <summary>
        /// 職位日期
        /// </summary>
        public DateTime? PositionDate { get; set; }
        /// <summary>
        /// 電話
        /// </summary>
        public string? CompPhone { get; set; }
        /// <summary>
        /// 分機
        /// </summary>
        public string Extension { get; set; } = null!;
        /// <summary>
        /// 國籍(CDE_TYPE=Nation_Id)
        /// </summary>
        public string? Nationality { get; set; }
        /// <summary>
        /// 血型
        /// </summary>
        public string? BloodType { get; set; }
        public string CrUser { get; set; } = null!;
        public DateTime CrDate { get; set; }
        public string? Userstamp { get; set; }
        public DateTime? Datestamp { get; set; }
        public string? WorkPlace { get; set; }
        public string? ErpId { get; set; }
        public string? Workers { get; set; }
        public string? Center { get; set; }
        public string Account { get; set; } = null!;
    }
}
