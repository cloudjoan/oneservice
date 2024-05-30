using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class 人事詳細資料view
    {
        public string Id { get; set; } = null!;
        public string? ErpId { get; set; }
        public string Pid { get; set; } = null!;
        public string? ParentId { get; set; }
        public string? DeptId { get; set; }
        public string? ParentName2 { get; set; }
        public string? DeptName2 { get; set; }
        public string? Name2 { get; set; }
        public string? Name { get; set; }
        public string? Birthday { get; set; }
        public string Sex { get; set; } = null!;
        public string SexDesc { get; set; } = null!;
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? CompPhone { get; set; }
        public string? Extension { get; set; }
        public string? RegestDate { get; set; }
        public string? CapitalPosition1 { get; set; }
        public string? WorkPlace { get; set; }
        public string? Cp1Nm { get; set; }
        public string? CapitalPosition2 { get; set; }
        public string? Cp2Nm { get; set; }
        public string? Position1 { get; set; }
        public string? Pt1Nm { get; set; }
        public string? Position2 { get; set; }
        public string? Pt2Nm { get; set; }
        public string? Constellation { get; set; }
        public string? ConstelDesc { get; set; }
        public string? Title1 { get; set; }
        public string? Title1Nm { get; set; }
        public string? Nationality { get; set; }
        public string? BloodType { get; set; }
        public string? CallInDate { get; set; }
        public string? CallOutDate { get; set; }
        public string? ReplaceDate { get; set; }
        public string? LeaveDate { get; set; }
        public string? LeaveReason { get; set; }
        public string? CompCde { get; set; }
        public string? CompDesc { get; set; }
        public string? AreaCde { get; set; }
        public string? AreaDesc { get; set; }
        public string? Title2 { get; set; }
        public string? JobType { get; set; }
        public string? JobDesc { get; set; }
        public string? CpDate { get; set; }
        public string? PositionDate { get; set; }
        public string? AssignType { get; set; }
        public string AtDesc { get; set; } = null!;
        public string? MilitaryService { get; set; }
        public string? MilitaryDesc { get; set; }
        public string? IsAutochthon { get; set; }
        public string AutochthonDesc { get; set; } = null!;
        public string? IsBodyBarrier { get; set; }
        public string BodyBarrierDesc { get; set; } = null!;
        public string? IsPhysicalHandbook { get; set; }
        public string PhysicalHandbookDesc { get; set; } = null!;
        public string? IsWelfareFund { get; set; }
        public string WelfareFundDesc { get; set; } = null!;
        public string? IsMarriage { get; set; }
        public string MarriageDesc { get; set; } = null!;
        public string? PhysicalDisDegree { get; set; }
        public int? EngReadingExamine { get; set; }
        public int? EngHearingExamine { get; set; }
        public int? EngNewToeflExamine { get; set; }
        public int? EngToeflExamine { get; set; }
        public int? EngToeicExamine { get; set; }
        public string? EngIeltsExamine { get; set; }
        public string? EngGeptExamine { get; set; }
        public string? EngDate { get; set; }
        public string? Expr1 { get; set; }
        public string? HZipCode { get; set; }
        public string? HAdd { get; set; }
        public string? HPhone { get; set; }
        public string? CZipCode { get; set; }
        public string? CAdd { get; set; }
        public string? CPhone { get; set; }
        public string? ContactPerson2 { get; set; }
        public string? ContactRelation2 { get; set; }
        public string? ContactPhone2 { get; set; }
        public string? ContactPerson1 { get; set; }
        public string? ContactRelation1 { get; set; }
        public string? ContactPhone1 { get; set; }
        public int? No { get; set; }
        public string? SchoolNm { get; set; }
        public string? MajorNm { get; set; }
        public string? DegreeCde { get; set; }
        public string? IsGraduate { get; set; }
        public string? SemesterStart { get; set; }
        public string? SemesterEnd { get; set; }
        public string? Division { get; set; }
        public string? DivisionDesc { get; set; }
        public string? DegreeDesc { get; set; }
        public string GraduateDesc { get; set; } = null!;
        public string? BirthPlace { get; set; }
        public string? BirthPlDesc { get; set; }
        public double Seniority { get; set; }
        public double TstiSeniority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ReinstatementDate { get; set; }
        public int? Age { get; set; }
        public string RetireDesc { get; set; } = null!;
        public string? ProfitCenterId { get; set; }
        public string? ProfitCenterName { get; set; }
        public string? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public string? Wt1Nm { get; set; }
        public string? Workers { get; set; }
        public string? Ct1Nm { get; set; }
        public string? Center { get; set; }
        public string? ManagerName { get; set; }
        public string? Expr2 { get; set; }
        public string? 公司名 { get; set; }
        public string? 部門名稱 { get; set; }
        public string? 人員姓名 { get; set; }
        public string? 職稱 { get; set; }
        public string? 帳號 { get; set; }
        public string? Expr3 { get; set; }
        public DateTime? 到職日期 { get; set; }
        public DateTime? 離職日期 { get; set; }
        public string? Expr4 { get; set; }
        public string? Ompostseriesname { get; set; }
        public string? Jobname { get; set; }
    }
}
