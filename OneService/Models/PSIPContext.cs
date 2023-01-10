using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneService.Models
{
    public partial class PSIPContext : DbContext
    {
        public PSIPContext()
        {
        }

        public PSIPContext(DbContextOptions<PSIPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TbBulletinItem> TbBulletinItems { get; set; } = null!;
        public virtual DbSet<TbOneOperationParameter> TbOneOperationParameters { get; set; } = null!;
        public virtual DbSet<TbOneRoleParameter> TbOneRoleParameters { get; set; } = null!;
        public virtual DbSet<TbOneSysParameter> TbOneSysParameters { get; set; } = null!;
        public virtual DbSet<TbProMilestone> TbProMilestones { get; set; } = null!;
        public virtual DbSet<TbProPjRecord> TbProPjRecords { get; set; } = null!;
        public virtual DbSet<TbProPjinfo> TbProPjinfos { get; set; } = null!;
        public virtual DbSet<TbProSupportEmp> TbProSupportEmps { get; set; } = null!;
        public virtual DbSet<TbProTask> TbProTasks { get; set; } = null!;
        public virtual DbSet<TbWorkingHoursMain> TbWorkingHoursMains { get; set; } = null!;
        public virtual DbSet<ViewProSupportEmp> ViewProSupportEmps { get; set; } = null!;
        public virtual DbSet<ViewWorkingHour> ViewWorkingHours { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.66;Database=PSIP;User=psip;Password=einck!@!NNd");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbBulletinItem>(entity =>
            {
                entity.HasKey(e => new { e.BulletinTypeId, e.BulletinItem });

                entity.ToTable("TB_BULLETIN_ITEM");

                entity.Property(e => e.BulletinTypeId)
                    .HasColumnName("bulletinTypeID")
                    .HasComment("ID");

                entity.Property(e => e.BulletinItem)
                    .HasColumnName("bulletinItem")
                    .HasComment("公告選項ID");

                entity.Property(e => e.BulletinContent)
                    .HasColumnName("bulletinContent")
                    .HasComment("預設內容");

                entity.Property(e => e.BulletinItemName)
                    .HasMaxLength(100)
                    .HasColumnName("bulletinItemName")
                    .HasComment("公告選項名稱");

                entity.Property(e => e.BulletinRemind)
                    .HasMaxLength(100)
                    .HasColumnName("bulletinRemind");

                entity.Property(e => e.BulletinSubject)
                    .HasMaxLength(100)
                    .HasColumnName("bulletinSubject")
                    .HasComment("預設主旨");

                entity.Property(e => e.DefaultTemplateId)
                    .HasColumnName("defaultTemplateID")
                    .HasComment("預設模板");

                entity.Property(e => e.IsApprove)
                    .HasColumnName("isApprove")
                    .HasComment("預設是否需簽核");

                entity.Property(e => e.IsEnabled)
                    .HasColumnName("isEnabled")
                    .HasComment("是否啟用");
            });

            modelBuilder.Entity<TbOneOperationParameter>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_OperationParameter");

                entity.Property(e => e.CId)
                    .ValueGeneratedNever()
                    .HasColumnName("cID");

                entity.Property(e => e.CModuleId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("cModuleID");

                entity.Property(e => e.COperationId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cOperationID");

                entity.Property(e => e.COperationName)
                    .HasMaxLength(100)
                    .HasColumnName("cOperationName");

                entity.Property(e => e.COperationUrl)
                    .HasMaxLength(500)
                    .HasColumnName("cOperationURL");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneRoleParameter>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_RoleParameter");

                entity.HasIndex(e => new { e.COperationId, e.CFunctionId, e.CCompanyId }, "NonClusteredIndex-20221019-103820");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CCompanyId)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("cCompanyID");

                entity.Property(e => e.CDescription)
                    .HasMaxLength(100)
                    .HasColumnName("cDescription");

                entity.Property(e => e.CExeDel)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cExeDel");

                entity.Property(e => e.CExeEdit)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cExeEdit");

                entity.Property(e => e.CExeInsert)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cExeInsert");

                entity.Property(e => e.CExeQuery)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cExeQuery");

                entity.Property(e => e.CFunctionId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cFunctionID");

                entity.Property(e => e.CIncludeSubDept)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIncludeSubDept");

                entity.Property(e => e.COperationId).HasColumnName("cOperationID");

                entity.Property(e => e.CValue)
                    .HasMaxLength(50)
                    .HasColumnName("cValue");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSysParameter>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SysParameter");

                entity.HasIndex(e => new { e.COperationId, e.CFunctionId, e.CCompanyId, e.CNo }, "NonClusteredIndex-20221019-103849");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CCompanyId)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("cCompanyID");

                entity.Property(e => e.CDescription)
                    .HasMaxLength(255)
                    .HasColumnName("cDescription");

                entity.Property(e => e.CFunctionId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cFunctionID");

                entity.Property(e => e.CNo)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("cNo");

                entity.Property(e => e.COperationId).HasColumnName("cOperationID");

                entity.Property(e => e.CValue)
                    .HasMaxLength(50)
                    .HasColumnName("cValue");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbProMilestone>(entity =>
            {
                entity.ToTable("TB_PRO_MILESTONE");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CrAccount).HasMaxLength(50);

                entity.Property(e => e.CrCompCode).HasMaxLength(10);

                entity.Property(e => e.CrDeptId).HasMaxLength(10);

                entity.Property(e => e.CrDeptName).HasMaxLength(50);

                entity.Property(e => e.CrEmail).HasMaxLength(100);

                entity.Property(e => e.CrErpId).HasMaxLength(8);

                entity.Property(e => e.CrName).HasMaxLength(50);

                entity.Property(e => e.CrmOppNo).HasMaxLength(10);

                entity.Property(e => e.DelayReason).HasMaxLength(500);

                entity.Property(e => e.EstimatedDate).HasMaxLength(22);

                entity.Property(e => e.FinishedDate).HasMaxLength(22);

                entity.Property(e => e.InsertTime).HasMaxLength(22);

                entity.Property(e => e.IsAlarm)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MilestoneNo).HasMaxLength(10);

                entity.Property(e => e.MsDescription).HasMaxLength(500);

                entity.Property(e => e.PaymentPeriod).HasMaxLength(10);

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.Tasks).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasMaxLength(22);

                entity.Property(e => e.WarningDays).HasMaxLength(10);
            });

            modelBuilder.Entity<TbProPjRecord>(entity =>
            {
                entity.ToTable("TB_PRO_PJ_RECORD");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Attachment).HasMaxLength(2000);

                entity.Property(e => e.Attendees).HasMaxLength(1000);

                entity.Property(e => e.BundleMs).HasMaxLength(10);

                entity.Property(e => e.BundleTask).HasMaxLength(10);

                entity.Property(e => e.CrAccount).HasMaxLength(50);

                entity.Property(e => e.CrCompCode).HasMaxLength(10);

                entity.Property(e => e.CrDeptId).HasMaxLength(10);

                entity.Property(e => e.CrDeptName).HasMaxLength(50);

                entity.Property(e => e.CrEmail).HasMaxLength(100);

                entity.Property(e => e.CrErpId).HasMaxLength(8);

                entity.Property(e => e.CrName).HasMaxLength(50);

                entity.Property(e => e.CrmOppNo).HasMaxLength(10);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.EndDatetime).HasMaxLength(22);

                entity.Property(e => e.ImplementedBy).HasMaxLength(500);

                entity.Property(e => e.Implementers).HasMaxLength(1000);

                entity.Property(e => e.InsertTime).HasMaxLength(22);

                entity.Property(e => e.Place).HasMaxLength(50);

                entity.Property(e => e.StartDatetime).HasMaxLength(22);

                entity.Property(e => e.TotalWorkHours).HasColumnType("numeric(6, 1)");

                entity.Property(e => e.UpdateTime).HasMaxLength(22);

                entity.Property(e => e.WithPpl).HasMaxLength(50);

                entity.Property(e => e.WithPplPhone).HasMaxLength(100);

                entity.Property(e => e.WorkHours).HasColumnType("numeric(6, 1)");
            });

            modelBuilder.Entity<TbProPjinfo>(entity =>
            {
                entity.ToTable("TB_PRO_PJINFO");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CrmOppNo).HasMaxLength(10);

                entity.Property(e => e.EndDate).HasMaxLength(22);

                entity.Property(e => e.EndDateUpdateTime).HasMaxLength(22);

                entity.Property(e => e.ExpEndDate).HasMaxLength(22);

                entity.Property(e => e.ExpEndDateUpdateTime).HasMaxLength(22);

                entity.Property(e => e.InsertTime).HasMaxLength(22);

                entity.Property(e => e.KickOffDate).HasMaxLength(22);

                entity.Property(e => e.KickOffDateUpdateTime).HasMaxLength(22);

                entity.Property(e => e.Pmacc)
                    .HasMaxLength(300)
                    .HasColumnName("PMAcc");

                entity.Property(e => e.PmaccUpdateTime)
                    .HasMaxLength(22)
                    .HasColumnName("PMAccUpdateTime");

                entity.Property(e => e.StartDate).HasMaxLength(22);

                entity.Property(e => e.StartDateUpdateTime).HasMaxLength(22);

                entity.Property(e => e.State).HasMaxLength(20);

                entity.Property(e => e.StateUpdateTime).HasMaxLength(22);
            });

            modelBuilder.Entity<TbProSupportEmp>(entity =>
            {
                entity.ToTable("TB_PRO_SUPPORT_EMP");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CrCompCode).HasMaxLength(10);

                entity.Property(e => e.CrDeptId).HasMaxLength(10);

                entity.Property(e => e.CrDeptName).HasMaxLength(50);

                entity.Property(e => e.CrEmail).HasMaxLength(100);

                entity.Property(e => e.CrErpId).HasMaxLength(8);

                entity.Property(e => e.CrName).HasMaxLength(50);

                entity.Property(e => e.CrmOppNo).HasMaxLength(10);

                entity.Property(e => e.InsertTime).HasMaxLength(22);

                entity.Property(e => e.PartnerId)
                    .HasMaxLength(10)
                    .HasColumnName("PARTNER_ID");

                entity.Property(e => e.SupCompCode).HasMaxLength(10);

                entity.Property(e => e.SupDeptId).HasMaxLength(10);

                entity.Property(e => e.SupDeptName).HasMaxLength(50);

                entity.Property(e => e.SupEmail).HasMaxLength(100);

                entity.Property(e => e.SupErpId).HasMaxLength(8);

                entity.Property(e => e.SupExt).HasMaxLength(20);

                entity.Property(e => e.SupMobile).HasMaxLength(20);

                entity.Property(e => e.SupName).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasMaxLength(22);
            });

            modelBuilder.Entity<TbProTask>(entity =>
            {
                entity.ToTable("TB_PRO_TASK");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActualDays).HasMaxLength(10);

                entity.Property(e => e.CrAccount).HasMaxLength(50);

                entity.Property(e => e.CrCompCode).HasMaxLength(10);

                entity.Property(e => e.CrDeptId).HasMaxLength(10);

                entity.Property(e => e.CrDeptName).HasMaxLength(50);

                entity.Property(e => e.CrEmail).HasMaxLength(100);

                entity.Property(e => e.CrErpId).HasMaxLength(8);

                entity.Property(e => e.CrName).HasMaxLength(50);

                entity.Property(e => e.CrmOppNo).HasMaxLength(10);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.EndDate).HasMaxLength(10);

                entity.Property(e => e.ExpDays).HasMaxLength(10);

                entity.Property(e => e.ExpEndDate).HasMaxLength(10);

                entity.Property(e => e.ExpStartDate).HasMaxLength(10);

                entity.Property(e => e.ExpWorkHours).HasColumnType("numeric(6, 1)");

                entity.Property(e => e.ImplementedBy).HasMaxLength(50);

                entity.Property(e => e.IncludeHoliday)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.InsertTime).HasMaxLength(22);

                entity.Property(e => e.Milestone).HasMaxLength(10);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PreWork).HasMaxLength(50);

                entity.Property(e => e.ProgressPercentage).HasMaxLength(10);

                entity.Property(e => e.StartDate).HasMaxLength(10);

                entity.Property(e => e.TaskState).HasMaxLength(10);

                entity.Property(e => e.UpdateTime).HasMaxLength(22);

                entity.Property(e => e.WorkHours).HasColumnType("numeric(6, 1)");
            });

            modelBuilder.Entity<TbWorkingHoursMain>(entity =>
            {
                entity.ToTable("TB_WorkingHoursMain");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActType).HasMaxLength(10);

                entity.Property(e => e.CrmOppName).HasMaxLength(200);

                entity.Property(e => e.CrmOppNo).HasMaxLength(10);

                entity.Property(e => e.Disabled).HasDefaultValueSql("((0))");

                entity.Property(e => e.EndTime).HasMaxLength(22);

                entity.Property(e => e.InsertTime).HasMaxLength(22);

                entity.Property(e => e.ModifyUser).HasMaxLength(30);

                entity.Property(e => e.StartTime).HasMaxLength(22);

                entity.Property(e => e.UpdateTime)
                    .HasMaxLength(22)
                    .HasColumnName("UpdateTIme");

                entity.Property(e => e.UserErpId).HasMaxLength(30);

                entity.Property(e => e.UserName).HasMaxLength(30);

                entity.Property(e => e.Whtype)
                    .HasMaxLength(10)
                    .HasColumnName("WHType");
            });

            modelBuilder.Entity<ViewProSupportEmp>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_PRO_SUPPORT_EMP");

                entity.Property(e => e.CrmOppNo).HasMaxLength(10);

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.SupErpId).HasMaxLength(8);
            });

            modelBuilder.Entity<ViewWorkingHour>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_WORKING_HOURS");

                entity.Property(e => e.ActType).HasMaxLength(10);

                entity.Property(e => e.ActTypeName).HasMaxLength(40);

                entity.Property(e => e.CrmOppName).HasMaxLength(200);

                entity.Property(e => e.FinishTime).HasMaxLength(22);

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.SourceFrom)
                    .HasMaxLength(11)
                    .IsUnicode(false);

                entity.Property(e => e.SrId).HasMaxLength(10);

                entity.Property(e => e.UserErpId).HasMaxLength(30);

                entity.Property(e => e.UserName).HasMaxLength(40);

                entity.Property(e => e.Whtype)
                    .HasMaxLength(10)
                    .HasColumnName("WHType");

                entity.Property(e => e.WhtypeName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("WHTypeName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
