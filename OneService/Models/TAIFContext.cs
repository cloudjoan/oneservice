using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneService.Models
{
    public partial class TAIFContext : DbContext
    {
        public TAIFContext()
        {
        }

        public TAIFContext(DbContextOptions<TAIFContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblEmployee> TblEmployees { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=TSTI-BPMDB;Database=TAIF;User=bpm;Password=70771557");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblEmployee>(entity =>
            {
                entity.HasKey(e => e.CEmployeeId);

                entity.ToTable("tblEmployee");

                entity.HasIndex(e => new { e.CEmployeeNo, e.CEmployeeErpid, e.CDepartmentId, e.CEmployeeCompanyCode }, "NonClusteredIndex-20220901-114852");

                entity.Property(e => e.CEmployeeId)
                    .ValueGeneratedNever()
                    .HasColumnName("cEmployee_ID");

                entity.Property(e => e.CDepartmentId).HasColumnName("cDepartment_ID");

                entity.Property(e => e.CEmployeeAccount)
                    .HasMaxLength(50)
                    .HasColumnName("cEmployee_Account");

                entity.Property(e => e.CEmployeeAreaCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_AreaCode");

                entity.Property(e => e.CEmployeeAssignType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Assign_Type");

                entity.Property(e => e.CEmployeeAway).HasColumnName("cEmployee_Away");

                entity.Property(e => e.CEmployeeBloodType)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_BloodType");

                entity.Property(e => e.CEmployeeBrithDay)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_BrithDay");

                entity.Property(e => e.CEmployeeCName)
                    .HasMaxLength(50)
                    .HasColumnName("cEmployee_cName");

                entity.Property(e => e.CEmployeeCallInDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_CallInDate");

                entity.Property(e => e.CEmployeeCallOutDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_CallOutDate");

                entity.Property(e => e.CEmployeeCapitalPosition1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_CapitalPosition1");

                entity.Property(e => e.CEmployeeCapitalPosition2)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_CapitalPosition2");

                entity.Property(e => e.CEmployeeCenter)
                    .HasMaxLength(20)
                    .HasColumnName("cEmployee_Center");

                entity.Property(e => e.CEmployeeCompanyCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_CompanyCode");

                entity.Property(e => e.CEmployeeCompanyPhone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_CompanyPhone");

                entity.Property(e => e.CEmployeeCompanyPhoneExt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_CompanyPhoneExt");

                entity.Property(e => e.CEmployeeConstellationId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_ConstellationID");

                entity.Property(e => e.CEmployeeCostCenterId).HasColumnName("cEmployee_CostCenterID");

                entity.Property(e => e.CEmployeeCpDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_CpDate");

                entity.Property(e => e.CEmployeeCreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_CreateDate");

                entity.Property(e => e.CEmployeeCreateUser).HasColumnName("cEmployee_CreateUser");

                entity.Property(e => e.CEmployeeEmail)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Email");

                entity.Property(e => e.CEmployeeEname)
                    .HasMaxLength(50)
                    .HasColumnName("cEmployee_Ename");

                entity.Property(e => e.CEmployeeErpid)
                    .HasMaxLength(10)
                    .HasColumnName("cEmployee_ERPID");

                entity.Property(e => e.CEmployeeFax)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Fax");

                entity.Property(e => e.CEmployeeIntroducer)
                    .HasMaxLength(50)
                    .HasColumnName("cEmployee_Introducer");

                entity.Property(e => e.CEmployeeIsEnable).HasColumnName("cEmployee_IsEnable");

                entity.Property(e => e.CEmployeeIsVegetarian).HasColumnName("cEmployee_IsVegetarian");

                entity.Property(e => e.CEmployeeJobStatusId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_JobStatusID");

                entity.Property(e => e.CEmployeeJobTypeId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_JobTypeID");

                entity.Property(e => e.CEmployeeLeaveDay)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("cEmployee_LeaveDay");

                entity.Property(e => e.CEmployeeLeaveReason)
                    .HasMaxLength(100)
                    .HasColumnName("cEmployee_LeaveReason");

                entity.Property(e => e.CEmployeeLevel).HasColumnName("cEmployee_Level");

                entity.Property(e => e.CEmployeeLevel1)
                    .HasMaxLength(50)
                    .HasColumnName("cEmployee_Level1");

                entity.Property(e => e.CEmployeeLevel1Date)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("cEmployee_Level1_Date");

                entity.Property(e => e.CEmployeeLevel2)
                    .HasMaxLength(50)
                    .HasColumnName("cEmployee_Level2");

                entity.Property(e => e.CEmployeeLevel2Date)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("cEmployee_Level2_Date");

                entity.Property(e => e.CEmployeeMobile)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Mobile");

                entity.Property(e => e.CEmployeeModifyDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_ModifyDate");

                entity.Property(e => e.CEmployeeModifyUser).HasColumnName("cEmployee_ModifyUser");

                entity.Property(e => e.CEmployeeNationalityId)
                    .HasMaxLength(10)
                    .HasColumnName("cEmployee_NationalityID");

                entity.Property(e => e.CEmployeeNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_NO");

                entity.Property(e => e.CEmployeeNotifyTypeId).HasColumnName("cEmployee_NotifyTypeID");

                entity.Property(e => e.CEmployeePhotoPath)
                    .HasMaxLength(200)
                    .HasColumnName("cEmployee_PhotoPath");

                entity.Property(e => e.CEmployeePid)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_PID");

                entity.Property(e => e.CEmployeePluralism).HasColumnName("cEmployee_Pluralism");

                entity.Property(e => e.CEmployeePosition1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Position1");

                entity.Property(e => e.CEmployeePosition2)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Position2");

                entity.Property(e => e.CEmployeePositionCode).HasColumnName("cEmployee_PositionCode");

                entity.Property(e => e.CEmployeePositionCode2)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_PositionCode2");

                entity.Property(e => e.CEmployeePositionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_PositionDate");

                entity.Property(e => e.CEmployeeProfitcenterId).HasColumnName("cEmployee_ProfitcenterID");

                entity.Property(e => e.CEmployeeRegestDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_RegestDate");

                entity.Property(e => e.CEmployeeReplaceDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEmployee_ReplaceDate");

                entity.Property(e => e.CEmployeeSexId)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_SexID");

                entity.Property(e => e.CEmployeeStatus).HasColumnName("cEmployee_Status");

                entity.Property(e => e.CEmployeeTakeDay)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("cEmployee_TakeDay");

                entity.Property(e => e.CEmployeeTel)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Tel");

                entity.Property(e => e.CEmployeeTitle)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Title");

                entity.Property(e => e.CEmployeeTitle1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Title1");

                entity.Property(e => e.CEmployeeTitle2)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cEmployee_Title2");

                entity.Property(e => e.CEmployeeWorkPlace)
                    .HasMaxLength(100)
                    .HasColumnName("cEmployee_WorkPlace");

                entity.Property(e => e.CEmployeeWorkers)
                    .HasMaxLength(50)
                    .HasColumnName("cEmployee_Workers");

                entity.Property(e => e.Pk)
                    .HasMaxLength(100)
                    .HasColumnName("pk");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
