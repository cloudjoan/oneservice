using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OneService.Models
{
    public partial class TSTIONEContext : DbContext
    {
        public TSTIONEContext()
        {
        }

        public TSTIONEContext(DbContextOptions<TSTIONEContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TbOneSridformat> TbOneSridformats { get; set; } = null!;
        public virtual DbSet<TbOneSrmain> TbOneSrmains { get; set; } = null!;
        public virtual DbSet<TbOneSrrepairType> TbOneSrrepairTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=PSIP-QAS;Database=TSTI-ONE;User=psip;Password=einck!@!NNd");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbOneSridformat>(entity =>
            {
                entity.HasKey(e => new { e.CId, e.CTitle, e.CYear, e.CMonth, e.CDay });

                entity.ToTable("TB_ONE_SRIDFormat");

                entity.Property(e => e.CId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("cID");

                entity.Property(e => e.CTitle)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("cTitle");

                entity.Property(e => e.CYear)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("cYear");

                entity.Property(e => e.CMonth)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("cMonth");

                entity.Property(e => e.CDay)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("cDay");

                entity.Property(e => e.CNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("cNO");
            });

            modelBuilder.Entity<TbOneSrmain>(entity =>
            {
                entity.HasKey(e => e.CSrid);

                entity.ToTable("TB_ONE_SRMain");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CAssEngineerId)
                    .HasMaxLength(255)
                    .HasColumnName("cAssEngineerID");

                entity.Property(e => e.CAttatchement).HasColumnName("cAttatchement");

                entity.Property(e => e.CContactAddress)
                    .HasMaxLength(110)
                    .HasColumnName("cContactAddress");

                entity.Property(e => e.CContactEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cContactEmail");

                entity.Property(e => e.CContactPhone)
                    .HasMaxLength(50)
                    .HasColumnName("cContactPhone");

                entity.Property(e => e.CContacterName)
                    .HasMaxLength(40)
                    .HasColumnName("cContacterName");

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(35)
                    .HasColumnName("cCustomerName");

                entity.Property(e => e.CDelayReason)
                    .HasMaxLength(255)
                    .HasColumnName("cDelayReason");

                entity.Property(e => e.CDesc)
                    .HasMaxLength(255)
                    .HasColumnName("cDesc");

                entity.Property(e => e.CMainEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cMainEngineerID");

                entity.Property(e => e.CMainEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cMainEngineerName");

                entity.Property(e => e.CMaserviceType)
                    .HasMaxLength(3)
                    .HasColumnName("cMAServiceType");

                entity.Property(e => e.CMaunit)
                    .HasMaxLength(4)
                    .HasColumnName("cMAUnit");

                entity.Property(e => e.CNotes)
                    .HasMaxLength(2000)
                    .HasColumnName("cNotes");

                entity.Property(e => e.CRepairName)
                    .HasMaxLength(40)
                    .HasColumnName("cRepairName");

                entity.Property(e => e.CSalesId)
                    .HasMaxLength(20)
                    .HasColumnName("cSalesID");

                entity.Property(e => e.CSalesName)
                    .HasMaxLength(40)
                    .HasColumnName("cSalesName");

                entity.Property(e => e.CSqperson)
                    .HasMaxLength(5)
                    .HasColumnName("cSQPerson");

                entity.Property(e => e.CSrpathWay)
                    .HasMaxLength(3)
                    .HasColumnName("cSRPathWay");

                entity.Property(e => e.CSrprocessWay)
                    .HasMaxLength(3)
                    .HasColumnName("cSRProcessWay");

                entity.Property(e => e.CSrtypeOne)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeOne");

                entity.Property(e => e.CSrtypeSec)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeSec");

                entity.Property(e => e.CSrtypeThr)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeThr");

                entity.Property(e => e.CStatus)
                    .HasMaxLength(5)
                    .HasColumnName("cStatus");

                entity.Property(e => e.CSystemGuid).HasColumnName("cSystemGUID");

                entity.Property(e => e.CTeamId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("cTeamID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrrepairType>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRRepairType");

                entity.HasIndex(e => e.CKindKey, "NonClusteredIndex-20221116-093929");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CKindKey)
                    .HasMaxLength(20)
                    .HasColumnName("cKIND_KEY");

                entity.Property(e => e.CKindLevel).HasColumnName("cKIND_LEVEL");

                entity.Property(e => e.CKindName)
                    .HasMaxLength(100)
                    .HasColumnName("cKIND_NAME");

                entity.Property(e => e.CKindNameEnUs)
                    .HasMaxLength(200)
                    .HasColumnName("cKIND_NAME_EN_US");

                entity.Property(e => e.CUpKindKey)
                    .HasMaxLength(20)
                    .HasColumnName("cUP_KIND_KEY");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
