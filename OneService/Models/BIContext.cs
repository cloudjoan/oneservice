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

        public virtual DbSet<MartAnalyseServiceRequestLabor> MartAnalyseServiceRequestLabors { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=TSTI-DW;Database=BI;User=biread;Password=!QAZ2wsx");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
