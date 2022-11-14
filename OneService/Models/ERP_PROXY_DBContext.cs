using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneService.Models
{
    public partial class ERP_PROXY_DBContext : DbContext
    {
        public ERP_PROXY_DBContext()
        {
        }

        public ERP_PROXY_DBContext(DbContextOptions<ERP_PROXY_DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<F4301codetail> F4301codetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.40;Database=ERP_PROXY_DB;User=sa;Password=Eip@dmin");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<F4301codetail>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.InternalNo });

                entity.ToTable("F4301CODetail");

                entity.Property(e => e.CompanyId)
                    .HasMaxLength(10)
                    .HasColumnName("CompanyID");

                entity.Property(e => e.InternalNo).HasMaxLength(15);

                entity.Property(e => e.Applicant).HasMaxLength(20);

                entity.Property(e => e.ApplyDate).HasMaxLength(20);

                entity.Property(e => e.ApproveDate).HasMaxLength(20);

                entity.Property(e => e.CostCenter).HasMaxLength(15);

                entity.Property(e => e.CostFormNo).HasMaxLength(30);

                entity.Property(e => e.CrmOppNo).HasMaxLength(20);

                entity.Property(e => e.FdeptId)
                    .HasMaxLength(30)
                    .HasColumnName("FDeptID");

                entity.Property(e => e.ImportDate).HasMaxLength(20);

                entity.Property(e => e.InternalExp).HasColumnName("InternalEXP");

                entity.Property(e => e.InternalType).HasMaxLength(15);

                entity.Property(e => e.OldInternalNo).HasMaxLength(30);

                entity.Property(e => e.OrderCost).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SapPurchasePrice).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SurplusBudget).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TotalPurchasePrice).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.WorkDateE)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("WorkDate_E");

                entity.Property(e => e.WorkDateS)
                    .HasMaxLength(20)
                    .HasColumnName("WorkDate_S");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
