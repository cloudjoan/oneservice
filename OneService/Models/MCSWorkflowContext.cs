using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneService.Models
{
    public partial class MCSWorkflowContext : DbContext
    {
        public MCSWorkflowContext()
        {
        }

        public MCSWorkflowContext(DbContextOptions<MCSWorkflowContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Person> People { get; set; } = null!;
        public virtual DbSet<TbServicesAppInstall> TbServicesAppInstalls { get; set; } = null!;
        public virtual DbSet<TbServicesAppInstalltemp> TbServicesAppInstalltemps { get; set; } = null!;
        public virtual DbSet<ViewDeptMgr> ViewDeptMgrs { get; set; } = null!;
        public virtual DbSet<ViewEmpInfo> ViewEmpInfos { get; set; } = null!;
        public virtual DbSet<ViewEmpInfoWithoutLeave> ViewEmpInfoWithoutLeaves { get; set; } = null!;
        public virtual DbSet<人事詳細資料view> 人事詳細資料views { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.26;Database=MCSWorkflow;User=eip;Password=70771557");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.HasIndex(e => new { e.ProfitCenterId, e.CompCde }, "NonClusteredIndex-20220901-112344");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ID")
                    .HasComment("部門代碼");

                entity.Property(e => e.CompCde)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Comp_Cde")
                    .IsFixedLength();

                entity.Property(e => e.CostCenterId)
                    .HasMaxLength(30)
                    .HasColumnName("CostCenterID");

                entity.Property(e => e.CrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CR_DATE");

                entity.Property(e => e.CrUser)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CR_USER");

                entity.Property(e => e.Datestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");

                entity.Property(e => e.DeptCode1)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.DeptCode2)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.DeptCode3)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.DeptCode4)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.DeptCode5)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("EMail");

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.FullName2).HasMaxLength(100);

                entity.Property(e => e.IsBusinessUnit).HasComment("BU");

                entity.Property(e => e.JdeDeptNm)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("JDE_Dept_Nm")
                    .HasComment("JDE部門名稱");

                entity.Property(e => e.JdeDeptNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("JDE_Dept_No")
                    .HasComment("JDE部門代碼");

                entity.Property(e => e.Level).HasComment("部門階層");

                entity.Property(e => e.LocationId)
                    .HasMaxLength(10)
                    .HasColumnName("LocationID");

                entity.Property(e => e.ManagerId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ManagerID")
                    .HasComment("主管員工編號");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Name2)
                    .HasMaxLength(50)
                    .HasComment("部門名稱(中文)");

                entity.Property(e => e.ParentId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ParentID")
                    .HasComment("上層部門代碼");

                entity.Property(e => e.PrintNum)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProfitCenterId)
                    .HasMaxLength(10)
                    .HasColumnName("ProfitCenterID");

                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USERSTAMP");

                entity.Property(e => e.VisitStoreUnit)
                    .HasMaxLength(36)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");

                entity.HasIndex(e => e.Pid, "IX_Person")
                    .IsUnique();

                entity.HasIndex(e => new { e.DeptId, e.ErpId, e.Account }, "NonClusteredIndex-20220901-112050");

                entity.Property(e => e.Id)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.Account)
                    .HasMaxLength(108)
                    .HasComputedColumnSql("(case charindex('@',[Email]) when (0) then '' else isnull('etatung\\'+left([Email],charindex('@',[Email])-(1)),'') end)", false);

                entity.Property(e => e.AgentEndTime).HasColumnType("datetime");

                entity.Property(e => e.AgentId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("AgentID");

                entity.Property(e => e.AgentStartTime)
                    .HasColumnType("datetime")
                    .HasComment("使用者編號(流水號)");

                entity.Property(e => e.Alias)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.AreaCde)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Area_Cde")
                    .IsFixedLength()
                    .HasComment("區域別");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasComment("生日");

                entity.Property(e => e.BloodType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("Blood_Type")
                    .IsFixedLength()
                    .HasComment("血型");

                entity.Property(e => e.CallInDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CallIn_Date")
                    .HasComment("調入日期");

                entity.Property(e => e.CallOutDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CallOut_Date")
                    .HasComment("調出日期");

                entity.Property(e => e.CapitalPosition1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Capital_Position1")
                    .IsFixedLength()
                    .HasComment("資位一(CDE_TYPE=CP1)");

                entity.Property(e => e.CapitalPosition2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Capital_Position2")
                    .IsFixedLength()
                    .HasComment("資位二(CDE_TYPE=CP2)");

                entity.Property(e => e.Center)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CompCde)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Comp_Cde")
                    .IsFixedLength()
                    .HasComment("公司別");

                entity.Property(e => e.CompPhone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Comp_Phone")
                    .HasComment("電話");

                entity.Property(e => e.Constellation)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasComment("星座(CDE_TYPE=CONSTEL)");

                entity.Property(e => e.CostCenter).HasMaxLength(20);

                entity.Property(e => e.CpDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Cp_Date")
                    .HasComment("資位日期");

                entity.Property(e => e.CrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CR_DATE");

                entity.Property(e => e.CrUser)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CR_USER");

                entity.Property(e => e.Datestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP");

                entity.Property(e => e.DeptId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DeptID")
                    .HasDefaultValueSql("('')")
                    .HasComment("所屬部門");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasComment("E-Mail");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(10)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.Extension)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')")
                    .HasComment("分機");

                entity.Property(e => e.JobStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Job_Status")
                    .HasDefaultValueSql("((1))")
                    .IsFixedLength()
                    .HasComment("任職現況");

                entity.Property(e => e.JobType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Job_Type")
                    .IsFixedLength()
                    .HasComment("職務(CDE_TYPE=JOB)");

                entity.Property(e => e.LeaveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Leave_Date")
                    .HasComment("離職日期");

                entity.Property(e => e.LeaveReason)
                    .HasMaxLength(100)
                    .HasColumnName("Leave_Reason");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(100)
                    .HasComment("手機");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('')")
                    .HasComment("英文姓名");

                entity.Property(e => e.Name2)
                    .HasMaxLength(100)
                    .HasComment("員工姓名");

                entity.Property(e => e.Nationality)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("國籍(CDE_TYPE=Nation_Id)");

                entity.Property(e => e.Phone).HasMaxLength(100);

                entity.Property(e => e.Pid)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PID")
                    .HasComment("員工編號(身份証字號)");

                entity.Property(e => e.Position1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("職位一(CDE_TYPE=PT1)");

                entity.Property(e => e.Position2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("職位二(CDE_TYPE=PT2)");

                entity.Property(e => e.PositionCode2)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.PositionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Position_Date")
                    .HasComment("職位日期");

                entity.Property(e => e.RegestDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Regest_Date")
                    .HasComment("到職日期");

                entity.Property(e => e.ReplaceDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Replace_Date")
                    .HasComment("最近一次復職日期");

                entity.Property(e => e.Sex)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("性別(M:男性、F:女性)");

                entity.Property(e => e.Title1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("職稱(CDE_TYPE=TITLE1)");

                entity.Property(e => e.Title2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("職種(CDE_TYPE=TITLE2)");

                entity.Property(e => e.TitleId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("TitleID")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.TitleName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USERSTAMP");

                entity.Property(e => e.WorkPlace)
                    .HasMaxLength(100)
                    .HasColumnName("Work_Place")
                    .IsFixedLength();

                entity.Property(e => e.Workers).HasMaxLength(50);
            });

            modelBuilder.Entity<TbServicesAppInstall>(entity =>
            {
                entity.ToTable("TB_SERVICES_APP_INSTALL");

                entity.HasIndex(e => e.Srid, "NonClusteredIndex-20230530-142545");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Account)
                    .HasMaxLength(50)
                    .HasColumnName("ACCOUNT");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(30)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(20)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.ExpectedDate).HasMaxLength(10);

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .HasColumnName("INSERT_TIME");

                entity.Property(e => e.InstallDate).HasMaxLength(10);

                entity.Property(e => e.InstallQuantity).HasColumnType("numeric(10, 0)");

                entity.Property(e => e.Srid)
                    .HasMaxLength(20)
                    .HasColumnName("SRID");

                entity.Property(e => e.TotalQuantity).HasColumnType("numeric(10, 0)");

                entity.Property(e => e.UpdateAccount)
                    .HasMaxLength(50)
                    .HasColumnName("UPDATE_ACCOUNT");

                entity.Property(e => e.UpdateEmpName)
                    .HasMaxLength(30)
                    .HasColumnName("UPDATE_EMP_NAME");

                entity.Property(e => e.UpdateTime)
                    .HasMaxLength(22)
                    .HasColumnName("UPDATE_TIME");
            });

            modelBuilder.Entity<TbServicesAppInstalltemp>(entity =>
            {
                entity.ToTable("TB_SERVICES_APP_INSTALLTEMP");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Account)
                    .HasMaxLength(50)
                    .HasColumnName("ACCOUNT");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(30)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(20)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.ExpectedDate).HasMaxLength(10);

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .HasColumnName("INSERT_TIME");

                entity.Property(e => e.InstallDate).HasMaxLength(10);

                entity.Property(e => e.InstallQuantity).HasColumnType("numeric(10, 0)");

                entity.Property(e => e.Srid)
                    .HasMaxLength(20)
                    .HasColumnName("SRID");

                entity.Property(e => e.TotalQuantity).HasColumnType("numeric(10, 0)");

                entity.Property(e => e.UpdateAccount)
                    .HasMaxLength(50)
                    .HasColumnName("UPDATE_ACCOUNT");

                entity.Property(e => e.UpdateEmpName)
                    .HasMaxLength(30)
                    .HasColumnName("UPDATE_EMP_NAME");

                entity.Property(e => e.UpdateTime)
                    .HasMaxLength(22)
                    .HasColumnName("UPDATE_TIME");
            });

            modelBuilder.Entity<ViewDeptMgr>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_DEPT_MGR");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE")
                    .IsFixedLength();

                entity.Property(e => e.DeptCode)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("DEPT_CODE");

                entity.Property(e => e.DeptLevel).HasColumnName("DEPT_LEVEL");

                entity.Property(e => e.DeptName)
                    .HasMaxLength(50)
                    .HasColumnName("DEPT_NAME");

                entity.Property(e => e.Disabled).HasColumnName("DISABLED");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(10)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.Up1DeptId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("UP1_DEPT_ID");

                entity.Property(e => e.Up1DeptMgErpId)
                    .HasMaxLength(10)
                    .HasColumnName("UP1_DEPT_MG_ERP_ID");

                entity.Property(e => e.Up1DeptName)
                    .HasMaxLength(50)
                    .HasColumnName("UP1_DEPT_NAME");

                entity.Property(e => e.Up1Level).HasColumnName("UP1_LEVEL");

                entity.Property(e => e.Up1Status).HasColumnName("UP1_Status");

                entity.Property(e => e.Up2DeptMgErpId)
                    .HasMaxLength(10)
                    .HasColumnName("UP2_DEPT_MG_ERP_ID");

                entity.Property(e => e.Up2DeptName)
                    .HasMaxLength(50)
                    .HasColumnName("UP2_DEPT_NAME");

                entity.Property(e => e.Up2DpetId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("UP2_DPET_ID");

                entity.Property(e => e.Up2Level).HasColumnName("UP2_LEVEL");

                entity.Property(e => e.Up2Status).HasColumnName("UP2_Status");
            });

            modelBuilder.Entity<ViewEmpInfo>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_EMP_INFO");

                entity.Property(e => e.Account)
                    .HasMaxLength(108)
                    .HasColumnName("ACCOUNT");

                entity.Property(e => e.CompName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_NAME");

                entity.Property(e => e.Constellation)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("CONSTELLATION");

                entity.Property(e => e.DeptId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DEPT_ID");

                entity.Property(e => e.DeptName)
                    .HasMaxLength(50)
                    .HasColumnName("DEPT_NAME");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(100)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(10)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.Id)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.RegistDate)
                    .HasColumnType("datetime")
                    .HasColumnName("REGIST_DATE");
            });

            modelBuilder.Entity<ViewEmpInfoWithoutLeave>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_EMP_INFO_WITHOUT_LEAVE");

                entity.Property(e => e.Account)
                    .HasMaxLength(108)
                    .HasColumnName("ACCOUNT");

                entity.Property(e => e.CompName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_NAME");

                entity.Property(e => e.Constellation)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("CONSTELLATION");

                entity.Property(e => e.DeptId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DEPT_ID");

                entity.Property(e => e.DeptName)
                    .HasMaxLength(50)
                    .HasColumnName("DEPT_NAME");

                entity.Property(e => e.EmpEname)
                    .HasMaxLength(100)
                    .HasColumnName("EMP_ENAME");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(100)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(10)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.Id)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.RegistDate)
                    .HasColumnType("datetime")
                    .HasColumnName("REGIST_DATE");
            });

            modelBuilder.Entity<人事詳細資料view>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("人事詳細資料view");

                entity.Property(e => e.AreaCde)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Area_Cde")
                    .IsFixedLength();

                entity.Property(e => e.AreaDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Area_Desc");

                entity.Property(e => e.AssignType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("Assign_Type")
                    .IsFixedLength();

                entity.Property(e => e.AtDesc)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("AT_Desc");

                entity.Property(e => e.AutochthonDesc)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("Autochthon_Desc");

                entity.Property(e => e.BirthPlDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("BirthPL_Desc");

                entity.Property(e => e.BirthPlace)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("Birth_Place")
                    .IsFixedLength();

                entity.Property(e => e.Birthday)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.BloodType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("Blood_Type")
                    .IsFixedLength();

                entity.Property(e => e.BodyBarrierDesc)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("Body_Barrier_Desc");

                entity.Property(e => e.CAdd)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("C_Add");

                entity.Property(e => e.CPhone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("C_Phone");

                entity.Property(e => e.CZipCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("C_Zip_Code");

                entity.Property(e => e.CallInDate)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CallIn_Date");

                entity.Property(e => e.CallOutDate)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("CallOut_Date");

                entity.Property(e => e.CapitalPosition1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Capital_Position1")
                    .IsFixedLength();

                entity.Property(e => e.CapitalPosition2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Capital_Position2")
                    .IsFixedLength();

                entity.Property(e => e.Center)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CompCde)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Comp_Cde")
                    .IsFixedLength();

                entity.Property(e => e.CompDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Comp_Desc");

                entity.Property(e => e.CompPhone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Comp_Phone");

                entity.Property(e => e.ConstelDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Constel_Desc");

                entity.Property(e => e.Constellation)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson1)
                    .HasMaxLength(20)
                    .HasColumnName("Contact_Person1");

                entity.Property(e => e.ContactPerson2)
                    .HasMaxLength(50)
                    .HasColumnName("Contact_Person2");

                entity.Property(e => e.ContactPhone1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Contact_Phone1");

                entity.Property(e => e.ContactPhone2)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("Contact_Phone2");

                entity.Property(e => e.ContactRelation1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Contact_relation1")
                    .IsFixedLength();

                entity.Property(e => e.ContactRelation2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Contact_relation2")
                    .IsFixedLength();

                entity.Property(e => e.CostCenterId)
                    .HasMaxLength(10)
                    .HasColumnName("CostCenterID");

                entity.Property(e => e.CostCenterName)
                    .HasMaxLength(40)
                    .HasColumnName("CostCenterNAME");

                entity.Property(e => e.Cp1Nm)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CP1_Nm");

                entity.Property(e => e.Cp2Nm)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CP2_Nm");

                entity.Property(e => e.CpDate)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Cp_Date");

                entity.Property(e => e.Ct1Nm)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CT1_Nm");

                entity.Property(e => e.DegreeCde)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Degree_Cde");

                entity.Property(e => e.DegreeDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Degree_Desc");

                entity.Property(e => e.DeptId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("DeptID");

                entity.Property(e => e.DeptName2)
                    .HasMaxLength(50)
                    .HasColumnName("Dept_Name2");

                entity.Property(e => e.Division)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DivisionDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Division_Desc");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("End_Date");

                entity.Property(e => e.EngDate)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Eng_Date");

                entity.Property(e => e.EngGeptExamine)
                    .HasMaxLength(50)
                    .HasColumnName("Eng_GEPT_examine");

                entity.Property(e => e.EngHearingExamine).HasColumnName("Eng_Hearing_examine");

                entity.Property(e => e.EngIeltsExamine)
                    .HasMaxLength(50)
                    .HasColumnName("Eng_IELTS_examine");

                entity.Property(e => e.EngNewToeflExamine).HasColumnName("Eng_NewTOEFL_examine");

                entity.Property(e => e.EngReadingExamine).HasColumnName("Eng_Reading_examine");

                entity.Property(e => e.EngToeflExamine).HasColumnName("Eng_TOEFL_examine");

                entity.Property(e => e.EngToeicExamine).HasColumnName("Eng_TOEIC_examine");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(10)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.Expr1)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Expr2)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Expr3)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Expr4)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Extension)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.GraduateDesc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("Graduate_Desc");

                entity.Property(e => e.HAdd)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("H_Add");

                entity.Property(e => e.HPhone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("H_Phone");

                entity.Property(e => e.HZipCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("H_Zip_Code");

                entity.Property(e => e.Id)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ID");

                entity.Property(e => e.IsAutochthon)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_Autochthon")
                    .IsFixedLength();

                entity.Property(e => e.IsBodyBarrier)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_Body_Barrier")
                    .IsFixedLength();

                entity.Property(e => e.IsGraduate)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_Graduate")
                    .IsFixedLength();

                entity.Property(e => e.IsMarriage)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_Marriage")
                    .IsFixedLength();

                entity.Property(e => e.IsPhysicalHandbook)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_Physical_Handbook")
                    .IsFixedLength();

                entity.Property(e => e.IsWelfareFund)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_Welfare_Fund")
                    .IsFixedLength();

                entity.Property(e => e.JobDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("JOB_Desc");

                entity.Property(e => e.JobType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Job_Type")
                    .IsFixedLength();

                entity.Property(e => e.Jobname)
                    .HasMaxLength(300)
                    .HasColumnName("jobname");

                entity.Property(e => e.LeaveDate)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Leave_Date");

                entity.Property(e => e.LeaveReason)
                    .HasMaxLength(100)
                    .HasColumnName("Leave_Reason");

                entity.Property(e => e.MajorNm)
                    .HasMaxLength(80)
                    .HasColumnName("Major_Nm");

                entity.Property(e => e.ManagerName).HasMaxLength(30);

                entity.Property(e => e.MarriageDesc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("Marriage_Desc");

                entity.Property(e => e.MilitaryDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Military_Desc");

                entity.Property(e => e.MilitaryService)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Military_Service")
                    .IsFixedLength();

                entity.Property(e => e.Mobile)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.Name2).HasMaxLength(30);

                entity.Property(e => e.Nationality)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Ompostseriesname)
                    .HasMaxLength(300)
                    .HasColumnName("OMpostseriesname")
                    .UseCollation("Chinese_PRC_CI_AS");

                entity.Property(e => e.ParentId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ParentID");

                entity.Property(e => e.ParentName2)
                    .HasMaxLength(50)
                    .HasColumnName("Parent_Name2");

                entity.Property(e => e.PhysicalDisDegree)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Physical_Dis_Degree")
                    .IsFixedLength();

                entity.Property(e => e.PhysicalHandbookDesc)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("Physical_Handbook_Desc");

                entity.Property(e => e.Pid)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PID");

                entity.Property(e => e.Position1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Position2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PositionDate)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Position_Date");

                entity.Property(e => e.ProfitCenterId)
                    .HasMaxLength(10)
                    .HasColumnName("ProfitCenterID");

                entity.Property(e => e.ProfitCenterName)
                    .HasMaxLength(40)
                    .HasColumnName("ProfitCenterNAME");

                entity.Property(e => e.Pt1Nm)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PT1_Nm");

                entity.Property(e => e.Pt2Nm)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PT2_Nm");

                entity.Property(e => e.RegestDate)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Regest_Date");

                entity.Property(e => e.ReinstatementDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Reinstatement_Date");

                entity.Property(e => e.ReplaceDate)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Replace_Date");

                entity.Property(e => e.RetireDesc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("Retire_Desc");

                entity.Property(e => e.SchoolNm)
                    .HasMaxLength(80)
                    .HasColumnName("SChool_Nm");

                entity.Property(e => e.SemesterEnd)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Semester_End");

                entity.Property(e => e.SemesterStart)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Semester_Start");

                entity.Property(e => e.Seniority).HasColumnName("seniority");

                entity.Property(e => e.Sex)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.SexDesc)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("Sex_Desc");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("Start_Date");

                entity.Property(e => e.Title1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Title1Nm)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Title1_Nm");

                entity.Property(e => e.Title2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.TstiSeniority).HasColumnName("tsti_Seniority");

                entity.Property(e => e.WelfareFundDesc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("Welfare_Fund_Desc");

                entity.Property(e => e.WorkPlace)
                    .HasMaxLength(100)
                    .HasColumnName("Work_Place")
                    .IsFixedLength();

                entity.Property(e => e.Workers).HasMaxLength(50);

                entity.Property(e => e.Wt1Nm)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WT1_Nm");

                entity.Property(e => e.人員姓名).HasMaxLength(30);

                entity.Property(e => e.公司名)
                    .HasMaxLength(12)
                    .IsUnicode(false);

                entity.Property(e => e.到職日期).HasColumnType("datetime");

                entity.Property(e => e.帳號)
                    .HasMaxLength(58)
                    .IsUnicode(false);

                entity.Property(e => e.職稱)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.部門名稱).HasMaxLength(50);

                entity.Property(e => e.離職日期).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
