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
        public virtual DbSet<TbPisInstallmaterial> TbPisInstallmaterials { get; set; } = null!;
        public virtual DbSet<TbProTask> TbProTasks { get; set; } = null!;

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

            modelBuilder.Entity<TbPisInstallmaterial>(entity =>
            {
                entity.HasKey(e => new { e.Srid, e.Srserial });

                entity.ToTable("TB_PIS_INSTALLMaterial");

                entity.Property(e => e.Srid)
                    .HasMaxLength(20)
                    .HasColumnName("SRID");

                entity.Property(e => e.Srserial)
                    .HasMaxLength(50)
                    .HasColumnName("SRSerial");

                entity.Property(e => e.MaterialId)
                    .HasMaxLength(100)
                    .HasColumnName("MaterialID");

                entity.Property(e => e.MaterialName).HasMaxLength(100);
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
