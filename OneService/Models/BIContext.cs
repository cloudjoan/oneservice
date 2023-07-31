using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneService.Models
{
    public partial class BIContext : DbContext
    {
        public BIContext()
        {
        }

        public BIContext(DbContextOptions<BIContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MartAnalyseFunnelBacklogRevenue> MartAnalyseFunnelBacklogRevenues { get; set; } = null!;
        public virtual DbSet<MartAnalyseServiceRequestLabor> MartAnalyseServiceRequestLabors { get; set; } = null!;
        public virtual DbSet<MartAnalyseSo> MartAnalyseSos { get; set; } = null!;
        public virtual DbSet<SdCostAnalysisHeader> SdCostAnalysisHeaders { get; set; } = null!;
        public virtual DbSet<SdOpphead> SdOppheads { get; set; } = null!;
        public virtual DbSet<SvSr> SvSrs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.94;Database=BI;User=biread;Password=!QAZ2wsx");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MartAnalyseFunnelBacklogRevenue>(entity =>
            {
                entity.ToTable("MART_AnalyseFunnelBacklogRevenue");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Account).HasMaxLength(20);

                entity.Property(e => e.Blamount).HasColumnName("BLAmount");

                entity.Property(e => e.Comp).HasMaxLength(10);

                entity.Property(e => e.Customer).HasMaxLength(100);

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.CustomerIdsales)
                    .HasMaxLength(10)
                    .HasColumnName("CustomerIDSales");

                entity.Property(e => e.CustomerSales).HasMaxLength(100);

                entity.Property(e => e.DealSize).HasMaxLength(50);

                entity.Property(e => e.DeptAnnualTargetPk)
                    .HasMaxLength(60)
                    .HasColumnName("DeptAnnualTargetPK");

                entity.Property(e => e.DeptCenter).HasMaxLength(50);

                entity.Property(e => e.DeptDepartment).HasMaxLength(50);

                entity.Property(e => e.DeptDivision).HasMaxLength(50);

                entity.Property(e => e.DeptId)
                    .HasMaxLength(100)
                    .HasColumnName("DeptID");

                entity.Property(e => e.DeptSection).HasMaxLength(50);

                entity.Property(e => e.DocNo).HasMaxLength(50);

                entity.Property(e => e.EmpAnnualOrderDateTargetPk)
                    .HasMaxLength(22)
                    .HasColumnName("EmpAnnualOrderDateTargetPK");

                entity.Property(e => e.EmpAnnualTargetPk)
                    .HasMaxLength(22)
                    .HasColumnName("EmpAnnualTargetPK");

                entity.Property(e => e.Employee).HasMaxLength(16);

                entity.Property(e => e.Funnel10).HasColumnName("Funnel 10%");

                entity.Property(e => e.Funnel10Profit).HasColumnName("Funnel 10% Profit");

                entity.Property(e => e.Funnel25).HasColumnName("Funnel 25%");

                entity.Property(e => e.Funnel25Profit).HasColumnName("Funnel 25% Profit");

                entity.Property(e => e.Funnel50).HasColumnName("Funnel 50%");

                entity.Property(e => e.Funnel50Profit).HasColumnName("Funnel 50% Profit");

                entity.Property(e => e.Funnel75).HasColumnName("Funnel 75%");

                entity.Property(e => e.Funnel75Profit).HasColumnName("Funnel 75% Profit");

                entity.Property(e => e.Funnel90).HasColumnName("Funnel 90%");

                entity.Property(e => e.Funnel90Profit).HasColumnName("Funnel 90% Profit");

                entity.Property(e => e.Industry).HasMaxLength(50);

                entity.Property(e => e.IndustryApplyFunction).HasMaxLength(100);

                entity.Property(e => e.IndustryApplyId)
                    .HasMaxLength(100)
                    .HasColumnName("IndustryApplyID");

                entity.Property(e => e.IndustryApplyIndustry).HasMaxLength(100);

                entity.Property(e => e.IndustryApplySolution).HasMaxLength(100);

                entity.Property(e => e.IndustrySales).HasMaxLength(50);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Internal15CostPs)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("Internal15CostPS");

                entity.Property(e => e.Internal15CostSwdev)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("Internal15CostSWDEV");

                entity.Property(e => e.Internal15CostSwmas)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("Internal15CostSWMAS");

                entity.Property(e => e.Internal15CosttiCc)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("Internal15CosttiCC");

                entity.Property(e => e.InternalNo).HasMaxLength(30);

                entity.Property(e => e.InvoiceDate).HasColumnType("datetime");

                entity.Property(e => e.Location).HasMaxLength(20);

                entity.Property(e => e.NextBlamount)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("NextBLAmount");

                entity.Property(e => e.OrderProfit).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.OrderRevenue).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.Pa)
                    .HasMaxLength(50)
                    .HasColumnName("PA");

                entity.Property(e => e.Paindustry)
                    .HasMaxLength(50)
                    .HasColumnName("PAIndustry");

                entity.Property(e => e.Patype)
                    .HasMaxLength(10)
                    .HasColumnName("PAType");

                entity.Property(e => e.Phase)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.PhaseBl)
                    .HasMaxLength(10)
                    .HasColumnName("PhaseBL");

                entity.Property(e => e.PhasePercent).HasMaxLength(10);

                entity.Property(e => e.PmtargetPk)
                    .HasMaxLength(70)
                    .HasColumnName("PMTargetPK");

                entity.Property(e => e.PreviousProfit).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.PreviousRevenue).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.ProdHierarchy).HasMaxLength(30);

                entity.Property(e => e.ProdHierarchyBrad).HasMaxLength(80);

                entity.Property(e => e.ProdId)
                    .HasMaxLength(50)
                    .HasColumnName("ProdID");

                entity.Property(e => e.Profit).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.ProfitCenter).HasMaxLength(10);

                entity.Property(e => e.Project).HasMaxLength(251);

                entity.Property(e => e.ProjectType).HasMaxLength(50);

                entity.Property(e => e.RecordType).HasMaxLength(10);

                entity.Property(e => e.Revenue).HasColumnType("decimal(18, 9)");

                entity.Property(e => e.Soamount)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("SOAmount");

                entity.Property(e => e.Soprofit)
                    .HasColumnType("decimal(18, 9)")
                    .HasColumnName("SOProfit");

                entity.Property(e => e.Sotype)
                    .HasMaxLength(80)
                    .HasColumnName("SOType");

                entity.Property(e => e.YearMonth).HasMaxLength(10);

                entity.Property(e => e.YearMonthOrderDate).HasMaxLength(10);
            });

            modelBuilder.Entity<MartAnalyseServiceRequestLabor>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("MART_AnalyseServiceRequestLabor");

                entity.Property(e => e.Contact).HasMaxLength(51);

                entity.Property(e => e.Customer).HasMaxLength(40);

                entity.Property(e => e.Description)
                    .HasMaxLength(40)
                    .HasColumnName("description");

                entity.Property(e => e.Engineer).HasMaxLength(40);

                entity.Property(e => e.EngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("EngineerID");

                entity.Property(e => e.Finish).HasMaxLength(20);

                entity.Property(e => e.Pk)
                    .HasMaxLength(4000)
                    .HasColumnName("PK");

                entity.Property(e => e.ProcessTypeText).HasMaxLength(40);

                entity.Property(e => e.Srid)
                    .HasMaxLength(10)
                    .HasColumnName("SRID");

                entity.Property(e => e.StatusText).HasMaxLength(40);
            });

            modelBuilder.Entity<MartAnalyseSo>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("MART_AnalyseSO");

                entity.Property(e => e.Account)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Blamount).HasColumnName("BLAmount");

                entity.Property(e => e.Comp).HasMaxLength(5);

                entity.Property(e => e.CreateDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Customer).HasMaxLength(35);

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.CustomerIdsales)
                    .HasMaxLength(10)
                    .HasColumnName("CustomerIDSales");

                entity.Property(e => e.CustomerSales).HasMaxLength(35);

                entity.Property(e => e.DeptAnnualTargetPk)
                    .HasMaxLength(57)
                    .HasColumnName("DeptAnnualTargetPK");

                entity.Property(e => e.DeptCenter).HasMaxLength(50);

                entity.Property(e => e.DeptDepartment).HasMaxLength(50);

                entity.Property(e => e.DeptDivision).HasMaxLength(50);

                entity.Property(e => e.DeptId)
                    .HasMaxLength(50)
                    .HasColumnName("DeptID");

                entity.Property(e => e.DeptSection).HasMaxLength(50);

                entity.Property(e => e.EmpAnnualOrderDateTargetPk)
                    .HasMaxLength(21)
                    .HasColumnName("EmpAnnualOrderDateTargetPK");

                entity.Property(e => e.EmpAnnualTargetPk)
                    .HasMaxLength(21)
                    .HasColumnName("EmpAnnualTargetPK");

                entity.Property(e => e.Employee).HasMaxLength(16);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Industry).HasMaxLength(50);

                entity.Property(e => e.IndustryApplyFunction).HasMaxLength(50);

                entity.Property(e => e.IndustryApplyId)
                    .HasMaxLength(20)
                    .HasColumnName("IndustryApplyID");

                entity.Property(e => e.IndustryApplyIndustry).HasMaxLength(100);

                entity.Property(e => e.IndustryApplySolution).HasMaxLength(100);

                entity.Property(e => e.IndustrySales).HasMaxLength(50);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.InternalNo).HasMaxLength(20);

                entity.Property(e => e.InvoiceDate).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Pa)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("PA");

                entity.Property(e => e.Paindustry)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("PAIndustry");

                entity.Property(e => e.Patype)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PAType");

                entity.Property(e => e.Phase)
                    .HasMaxLength(7)
                    .IsUnicode(false);

                entity.Property(e => e.PhaseBl)
                    .HasMaxLength(8)
                    .HasColumnName("PhaseBL");

                entity.Property(e => e.PhasePercent)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PmtargetPk)
                    .HasMaxLength(20)
                    .HasColumnName("PMTargetPK");

                entity.Property(e => e.ProdHierarchy).HasMaxLength(18);

                entity.Property(e => e.ProdHierarchyBrad).HasMaxLength(3);

                entity.Property(e => e.ProdId)
                    .HasMaxLength(18)
                    .HasColumnName("ProdID");

                entity.Property(e => e.ProfitCenter).HasMaxLength(10);

                entity.Property(e => e.Project)
                    .HasMaxLength(50)
                    .HasColumnName("project");

                entity.Property(e => e.Quantity).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.RecordType)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ShipType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("shipType");

                entity.Property(e => e.Soamount).HasColumnName("SOAmount");

                entity.Property(e => e.Soitem)
                    .HasMaxLength(10)
                    .HasColumnName("SOItem");

                entity.Property(e => e.Sono)
                    .HasMaxLength(10)
                    .HasColumnName("SONo");

                entity.Property(e => e.Sopk)
                    .HasMaxLength(20)
                    .HasColumnName("SOPK");

                entity.Property(e => e.Soprofit)
                    .HasColumnType("numeric(35, 9)")
                    .HasColumnName("SOProfit");

                entity.Property(e => e.Sotype)
                    .HasMaxLength(80)
                    .HasColumnName("SOType");

                entity.Property(e => e.YearMonth).HasMaxLength(7);

                entity.Property(e => e.YearMonthOrderDate).HasMaxLength(7);
            });

            modelBuilder.Entity<SdCostAnalysisHeader>(entity =>
            {
                entity.HasKey(e => new { e.FormNo, e.Year });

                entity.ToTable("SD_CostAnalysisHeader");

                entity.Property(e => e.FormNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Year)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.ApplyDate).HasColumnType("datetime");

                entity.Property(e => e.Bg)
                    .HasMaxLength(4)
                    .HasColumnName("BG");

                entity.Property(e => e.Company)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.ContractType).HasMaxLength(10);

                entity.Property(e => e.CostType).HasMaxLength(22);

                entity.Property(e => e.CrmOppNo).HasMaxLength(20);

                entity.Property(e => e.Customer).HasMaxLength(200);

                entity.Property(e => e.CustomerGroup).HasMaxLength(80);

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(20)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.DealDate).HasMaxLength(22);

                entity.Property(e => e.DeptId)
                    .HasMaxLength(100)
                    .HasColumnName("DeptID");

                entity.Property(e => e.Employee).HasMaxLength(50);

                entity.Property(e => e.EmployeeErpid)
                    .HasMaxLength(20)
                    .HasColumnName("EmployeeERPID");

                entity.Property(e => e.EmployeeNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EmployeeNO");

                entity.Property(e => e.EstimateDate).HasMaxLength(22);

                entity.Property(e => e.EstimateProfit).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.EstimateProfitInInter)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.FullDepartmentName).HasMaxLength(150);

                entity.Property(e => e.Industry).HasMaxLength(50);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.InsertUser).HasMaxLength(20);

                entity.Property(e => e.Installment).HasMaxLength(2);

                entity.Property(e => e.InternalNo).HasMaxLength(30);

                entity.Property(e => e.InvoiceDate).HasMaxLength(22);

                entity.Property(e => e.InvoiceYear).HasMaxLength(20);

                entity.Property(e => e.Location).HasMaxLength(50);

                entity.Property(e => e.OppPhasePercent).HasMaxLength(50);

                entity.Property(e => e.OppType).HasMaxLength(16);

                entity.Property(e => e.Pa)
                    .HasMaxLength(40)
                    .HasColumnName("PA");

                entity.Property(e => e.Paindustry)
                    .HasMaxLength(30)
                    .HasColumnName("PAIndustry");

                entity.Property(e => e.Patype)
                    .HasMaxLength(50)
                    .HasColumnName("PAType");

                entity.Property(e => e.PreProfit).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.PreorderAmount).HasColumnName("preorderAmount");

                entity.Property(e => e.PreorderProfit).HasColumnName("preorderProfit");

                entity.Property(e => e.ProductSales).HasMaxLength(10);

                entity.Property(e => e.ProfitCenterId)
                    .HasMaxLength(20)
                    .HasColumnName("ProfitCenterID");

                entity.Property(e => e.Project).HasMaxLength(251);

                entity.Property(e => e.ProjectType).HasMaxLength(50);

                entity.Property(e => e.Sn).HasColumnName("sn");

                entity.Property(e => e.StartDate).HasMaxLength(22);

                entity.Property(e => e.TotalCost).HasDefaultValueSql("((0))");

                entity.Property(e => e.TotalPrice).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TotalPriceAddTax)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("TotalPrice_AddTax");

                entity.Property(e => e.TotalPriceAll)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("TotalPrice_All");

                entity.Property(e => e.TotalPriceWithout15).HasColumnName("TotalPrice_Without15");
            });

            modelBuilder.Entity<SdOpphead>(entity =>
            {
                entity.HasKey(e => e.OppNo)
                    .HasName("PK_SO_OPPHead");

                entity.ToTable("SD_OPPHead");

                entity.Property(e => e.OppNo)
                    .HasMaxLength(10)
                    .HasComment("商機編號");

                entity.Property(e => e.CancelReason).HasMaxLength(200);

                entity.Property(e => e.Commit).HasMaxLength(10);

                entity.Property(e => e.CompId)
                    .HasMaxLength(4)
                    .HasColumnName("CompID");

                entity.Property(e => e.ContractType).HasMaxLength(20);

                entity.Property(e => e.Customer)
                    .HasMaxLength(100)
                    .HasComment("客戶");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("CustomerID")
                    .HasComment("客戶編號");

                entity.Property(e => e.DeptId)
                    .HasMaxLength(50)
                    .HasColumnName("DeptID");

                entity.Property(e => e.EmployeeId)
                    .HasMaxLength(15)
                    .HasColumnName("EmployeeID")
                    .HasComment("員工編號");

                entity.Property(e => e.FormNo).HasMaxLength(50);

                entity.Property(e => e.Industry).HasMaxLength(100);

                entity.Property(e => e.IndustryApplyFunctionId)
                    .HasMaxLength(100)
                    .HasColumnName("IndustryApplyFunctionID");

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.InsertUser).HasMaxLength(20);

                entity.Property(e => e.InternalNo).HasMaxLength(30);

                entity.Property(e => e.InternalNoAreadySo).HasColumnName("InternalNoAreadySO");

                entity.Property(e => e.InvoiceCommit).HasMaxLength(5);

                entity.Property(e => e.InvoiceDate)
                    .HasColumnType("datetime")
                    .HasComment("發票日期");

                entity.Property(e => e.OppCurrency)
                    .HasMaxLength(10)
                    .HasComment("幣別");

                entity.Property(e => e.OppDesc)
                    .HasMaxLength(200)
                    .HasComment("商機描述");

                entity.Property(e => e.OppEndDate)
                    .HasColumnType("datetime")
                    .HasComment("結案日期");

                entity.Property(e => e.OppEstimateDate)
                    .HasColumnType("datetime")
                    .HasComment("出貨日期");

                entity.Property(e => e.OppInsertTime)
                    .HasColumnType("datetime")
                    .HasComment("商機輸入日期");

                entity.Property(e => e.OppOrgChannel).HasMaxLength(2);

                entity.Property(e => e.OppOrgDivision).HasMaxLength(2);

                entity.Property(e => e.OppOrgGroup).HasMaxLength(12);

                entity.Property(e => e.OppOrgOffice).HasMaxLength(12);

                entity.Property(e => e.OppOrgResp).HasMaxLength(12);

                entity.Property(e => e.OppOrgShort).HasMaxLength(12);

                entity.Property(e => e.OppPhase)
                    .HasMaxLength(10)
                    .HasComment("商機階段代碼");

                entity.Property(e => e.OppPhaseDesc)
                    .HasMaxLength(40)
                    .HasComment("商機階段說明");

                entity.Property(e => e.OppPhasePercent)
                    .HasMaxLength(10)
                    .HasComment("商機階段");

                entity.Property(e => e.OppProfit).HasComment("商機毛利");

                entity.Property(e => e.OppRevenue).HasComment("商機金額");

                entity.Property(e => e.OppSalesCycle)
                    .HasMaxLength(10)
                    .HasComment("銷售循環代碼");

                entity.Property(e => e.OppSalesCycleDesc)
                    .HasMaxLength(40)
                    .HasComment("銷售循環");

                entity.Property(e => e.OppStartDate)
                    .HasColumnType("datetime")
                    .HasComment("起始日起");

                entity.Property(e => e.OppStatus)
                    .HasMaxLength(20)
                    .HasComment("商機狀態代碼");

                entity.Property(e => e.OppStatusDesc)
                    .HasMaxLength(40)
                    .HasComment("商機狀態");

                entity.Property(e => e.OppType).HasMaxLength(50);

                entity.Property(e => e.ProductSales).HasMaxLength(20);

                entity.Property(e => e.ProfitCenter).HasMaxLength(10);

                entity.Property(e => e.Psostage)
                    .HasMaxLength(6)
                    .HasColumnName("PSOStage")
                    .HasComment("PSO支援狀態");

                entity.Property(e => e.YearMonth)
                    .HasMaxLength(7)
                    .HasComment("年度月份");

                entity.Property(e => e.YearMonthOrderDate).HasMaxLength(7);
            });

            modelBuilder.Entity<SvSr>(entity =>
            {
                entity.ToTable("SV_SR");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Address).HasMaxLength(60);

                entity.Property(e => e.Arrive).HasMaxLength(20);

                entity.Property(e => e.Backupsn)
                    .HasMaxLength(300)
                    .HasColumnName("BACKUPSN");

                entity.Property(e => e.Changepart)
                    .HasMaxLength(1000)
                    .HasColumnName("CHANGEPART");

                entity.Property(e => e.Contact).HasMaxLength(40);

                entity.Property(e => e.ContactId)
                    .HasMaxLength(10)
                    .HasColumnName("ContactID");

                entity.Property(e => e.Contract).HasMaxLength(20);

                entity.Property(e => e.Countin).HasMaxLength(20);

                entity.Property(e => e.Countout).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasMaxLength(20);

                entity.Property(e => e.Csrid)
                    .HasMaxLength(40)
                    .HasColumnName("CSRID");

                entity.Property(e => e.Csrlink)
                    .HasMaxLength(100)
                    .HasColumnName("CSRLINK");

                entity.Property(e => e.Customer).HasMaxLength(40);

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.Dealway).HasMaxLength(20);

                entity.Property(e => e.Depart).HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(40);

                entity.Property(e => e.Dn)
                    .HasMaxLength(20)
                    .HasColumnName("DN");

                entity.Property(e => e.Email).HasMaxLength(250);

                entity.Property(e => e.Engineer).HasMaxLength(40);

                entity.Property(e => e.EngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("EngineerID");

                entity.Property(e => e.Finish).HasMaxLength(20);

                entity.Property(e => e.Hardware).HasMaxLength(2);

                entity.Property(e => e.Hpct)
                    .HasMaxLength(1000)
                    .HasColumnName("HPCT");

                entity.Property(e => e.Hpxc)
                    .HasMaxLength(300)
                    .HasColumnName("HPXC");

                entity.Property(e => e.InserUser).HasMaxLength(20);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Labor).HasMaxLength(20);

                entity.Property(e => e.Mobile).HasMaxLength(30);

                entity.Property(e => e.Newct)
                    .HasMaxLength(1000)
                    .HasColumnName("NEWCT");

                entity.Property(e => e.Oa)
                    .HasMaxLength(20)
                    .HasColumnName("OA");

                entity.Property(e => e.Oldct)
                    .HasMaxLength(1000)
                    .HasColumnName("OLDCT");

                entity.Property(e => e.Other).HasMaxLength(2);

                entity.Property(e => e.Pid)
                    .HasMaxLength(40)
                    .HasColumnName("PID");

                entity.Property(e => e.Pn)
                    .HasMaxLength(40)
                    .HasColumnName("PN");

                entity.Property(e => e.Problem).HasMaxLength(2048);

                entity.Property(e => e.ProcessType).HasMaxLength(4);

                entity.Property(e => e.ProcessTypeText).HasMaxLength(40);

                entity.Property(e => e.Reason).HasMaxLength(60);

                entity.Property(e => e.Refix).HasMaxLength(2);

                entity.Property(e => e.Replace)
                    .HasMaxLength(2)
                    .HasColumnName("Replace_");

                entity.Property(e => e.Reset).HasMaxLength(2);

                entity.Property(e => e.Restore)
                    .HasMaxLength(2)
                    .HasColumnName("Restore_");

                entity.Property(e => e.Rkind1)
                    .HasMaxLength(40)
                    .HasColumnName("RKIND1");

                entity.Property(e => e.Rkind2)
                    .HasMaxLength(40)
                    .HasColumnName("RKIND2");

                entity.Property(e => e.Rkind3)
                    .HasMaxLength(40)
                    .HasColumnName("RKIND3");

                entity.Property(e => e.Sales).HasMaxLength(40);

                entity.Property(e => e.Shop).HasMaxLength(40);

                entity.Property(e => e.Slaresp)
                    .HasMaxLength(20)
                    .HasColumnName("SLARESP");

                entity.Property(e => e.Slasrv)
                    .HasMaxLength(20)
                    .HasColumnName("SLASRV");

                entity.Property(e => e.Sn)
                    .HasMaxLength(40)
                    .HasColumnName("SN");

                entity.Property(e => e.So)
                    .HasMaxLength(20)
                    .HasColumnName("SO");

                entity.Property(e => e.Software).HasMaxLength(2);

                entity.Property(e => e.Solution).HasMaxLength(2048);

                entity.Property(e => e.Sq)
                    .HasMaxLength(40)
                    .HasColumnName("SQ");

                entity.Property(e => e.Srid)
                    .HasMaxLength(10)
                    .HasColumnName("SRID");

                entity.Property(e => e.Srvkind)
                    .HasMaxLength(20)
                    .HasColumnName("SRVKind");

                entity.Property(e => e.Srvteam)
                    .HasMaxLength(50)
                    .HasColumnName("SRVTeam");

                entity.Property(e => e.Status).HasMaxLength(5);

                entity.Property(e => e.StatusText).HasMaxLength(40);

                entity.Property(e => e.Tel).HasMaxLength(30);

                entity.Property(e => e.Unit).HasMaxLength(20);

                entity.Property(e => e.Wtydesc)
                    .HasMaxLength(40)
                    .HasColumnName("WTYDesc");

                entity.Property(e => e.Wtyend)
                    .HasMaxLength(20)
                    .HasColumnName("WTYEnd");

                entity.Property(e => e.Wtyid)
                    .HasMaxLength(40)
                    .HasColumnName("WTYID");

                entity.Property(e => e.Wtystart)
                    .HasMaxLength(20)
                    .HasColumnName("WTYStart");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
