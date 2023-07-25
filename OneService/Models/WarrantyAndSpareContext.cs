using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneService.Models
{
    public partial class WarrantyAndSpareContext : DbContext
    {
        public WarrantyAndSpareContext()
        {
        }

        public WarrantyAndSpareContext(DbContextOptions<WarrantyAndSpareContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SystemFormFlowParameter> SystemFormFlowParameters { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.66;Database=WarrantyAndSpare;User=psip;Password=einck!@!NNd");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemFormFlowParameter>(entity =>
            {
                entity.HasKey(e => new { e.CFunctionNo, e.CNo });

                entity.ToTable("SystemFormFlowParameter");

                entity.HasIndex(e => new { e.CFunctionNo, e.CNo, e.CType }, "NonClusteredIndex-20230210-093100");

                entity.Property(e => e.CFunctionNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cFunctionNo");

                entity.Property(e => e.CNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cNo");

                entity.Property(e => e.CCreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cCreatedDate");

                entity.Property(e => e.CCreatedUser).HasColumnName("cCreatedUser");

                entity.Property(e => e.CDescription)
                    .HasMaxLength(200)
                    .HasColumnName("cDescription");

                entity.Property(e => e.CModifiedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cModifiedDate");

                entity.Property(e => e.CModifiedUser).HasColumnName("cModifiedUser");

                entity.Property(e => e.CType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cType");

                entity.Property(e => e.CValue)
                    .HasMaxLength(200)
                    .HasColumnName("cValue");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
