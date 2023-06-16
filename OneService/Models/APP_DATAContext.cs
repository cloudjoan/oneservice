using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneService.Models
{
    public partial class APP_DATAContext : DbContext
    {
        public APP_DATAContext()
        {
        }

        public APP_DATAContext(DbContextOptions<APP_DATAContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TbAccessHistory> TbAccessHistories { get; set; } = null!;
        public virtual DbSet<TbAccessable> TbAccessables { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.26;Database=APP_DATA;User=eip;Password=70771557");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbAccessHistory>(entity =>
            {
                entity.ToTable("TB_ACCESS_HISTORY");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccessLocation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ACCESS_LOCATION");

                entity.Property(e => e.AccessResulst)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ACCESS_RESULST");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .IsUnicode(false)
                    .HasColumnName("INSERT_TIME");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("USER_NAME");
            });

            modelBuilder.Entity<TbAccessable>(entity =>
            {
                entity.ToTable("TB_ACCESSABLE");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AccessLocation)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ACCESS_LOCATION");

                entity.Property(e => e.ErpId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ERP_ID");

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .IsUnicode(false)
                    .HasColumnName("INSERT_TIME");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("USER_NAME");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
