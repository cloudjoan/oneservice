﻿using System;
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

        public virtual DbSet<TblDepartment> TblDepartments { get; set; } = null!;
        public virtual DbSet<TblEmployee> TblEmployees { get; set; } = null!;
        public virtual DbSet<TblFormGuaranteePop> TblFormGuaranteePops { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.50;Database=TAIF;User=BPM;Password=70771557");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblDepartment>(entity =>
            {
                entity.HasKey(e => e.CDepartmentId);

                entity.ToTable("tblDepartment");

                entity.HasIndex(e => new { e.Pk, e.CDepartmentParentId, e.CDepartmentManager }, "NonClusteredIndex-20220901-115038");

                entity.Property(e => e.CDepartmentId)
                    .ValueGeneratedNever()
                    .HasColumnName("cDepartment_ID");

                entity.Property(e => e.CCrossDeptId).HasColumnName("cCrossDept_ID");

                entity.Property(e => e.CDepartmentCreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cDepartment_CreateDate");

                entity.Property(e => e.CDepartmentCreateUser).HasColumnName("cDepartment_CreateUser");

                entity.Property(e => e.CDepartmentDisplayOrder).HasColumnName("cDepartment_DisplayOrder");

                entity.Property(e => e.CDepartmentEname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cDepartment_EName");

                entity.Property(e => e.CDepartmentIsBusinessUnit).HasColumnName("cDepartment_IsBusinessUnit");

                entity.Property(e => e.CDepartmentIsCostCenter).HasColumnName("cDepartment_IsCostCenter");

                entity.Property(e => e.CDepartmentIsEnable).HasColumnName("cDepartment_IsEnable");

                entity.Property(e => e.CDepartmentIsVirtual)
                    .HasColumnName("cDepartment_IsVirtual")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.CDepartmentLevel).HasColumnName("cDepartment_Level");

                entity.Property(e => e.CDepartmentLocationId)
                    .HasMaxLength(10)
                    .HasColumnName("cDepartment_LocationID");

                entity.Property(e => e.CDepartmentManager).HasColumnName("cDepartment_Manager");

                entity.Property(e => e.CDepartmentModifyDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cDepartment_ModifyDate");

                entity.Property(e => e.CDepartmentModifyUser).HasColumnName("cDepartment_ModifyUser");

                entity.Property(e => e.CDepartmentName)
                    .HasMaxLength(50)
                    .HasColumnName("cDepartment_Name");

                entity.Property(e => e.CDepartmentNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("cDepartment_NO");

                entity.Property(e => e.CDepartmentParentId).HasColumnName("cDepartment_ParentID");

                entity.Property(e => e.CDepartmentStatus)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("cDepartment_Status");

                entity.Property(e => e.CDisabledUpdate)
                    .HasColumnName("cDisabled_Update")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Pk)
                    .HasMaxLength(100)
                    .HasColumnName("pk");
            });

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

            modelBuilder.Entity<TblFormGuaranteePop>(entity =>
            {
                entity.HasKey(e => e.CFormNo)
                    .HasName("PK__tblForm___FE82A1B27132C993");

                entity.ToTable("tblForm_Guarantee_Pop");

                entity.Property(e => e.CFormNo)
                    .HasMaxLength(50)
                    .HasColumnName("cFormNo");

                entity.Property(e => e.CApplyEmail)
                    .HasMaxLength(100)
                    .HasColumnName("cApplyEmail");

                entity.Property(e => e.CApplyName)
                    .HasMaxLength(50)
                    .HasColumnName("cApplyName");

                entity.Property(e => e.CApplyTel)
                    .HasMaxLength(30)
                    .HasColumnName("cApplyTel");

                entity.Property(e => e.CCounts)
                    .HasMaxLength(10)
                    .HasColumnName("cCounts");

                entity.Property(e => e.CCustAddr)
                    .HasMaxLength(200)
                    .HasColumnName("cCustAddr");

                entity.Property(e => e.CCustComp)
                    .HasMaxLength(100)
                    .HasColumnName("cCustComp");

                entity.Property(e => e.CCustEmail)
                    .HasMaxLength(100)
                    .HasColumnName("cCustEmail");

                entity.Property(e => e.CCustName)
                    .HasMaxLength(100)
                    .HasColumnName("cCustName");

                entity.Property(e => e.CCustTel)
                    .HasMaxLength(30)
                    .HasColumnName("cCustTel");

                entity.Property(e => e.COrgWarrantyDate)
                    .HasMaxLength(22)
                    .HasColumnName("cOrgWarrantyDate");

                entity.Property(e => e.CProdName)
                    .HasMaxLength(100)
                    .HasColumnName("cProdName");

                entity.Property(e => e.CProdSn)
                    .HasMaxLength(200)
                    .HasColumnName("cProdSN");

                entity.Property(e => e.CPurpose)
                    .HasMaxLength(2000)
                    .HasColumnName("cPurpose");

                entity.Property(e => e.CReceiptDate)
                    .HasMaxLength(22)
                    .HasColumnName("cReceiptDate");

                entity.Property(e => e.CReceiptNo)
                    .HasMaxLength(100)
                    .HasColumnName("cReceiptNo");

                entity.Property(e => e.CWarrantyDate)
                    .HasMaxLength(22)
                    .HasColumnName("cWarrantyDate");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
