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

        public virtual DbSet<TbLuckydraw> TbLuckydraws { get; set; } = null!;
        public virtual DbSet<TbLuckydrawPrize> TbLuckydrawPrizes { get; set; } = null!;
        public virtual DbSet<TbLuckydrawPrizewinning> TbLuckydrawPrizewinnings { get; set; } = null!;

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
            modelBuilder.Entity<TbLuckydraw>(entity =>
            {
                entity.HasKey(e => e.DrawId)
                    .HasName("PK__TB_LUCKY__4F0302CA2704CA5F");

                entity.ToTable("TB_LUCKYDRAW");

                entity.Property(e => e.DrawId).HasColumnName("Draw_ID");

                entity.Property(e => e.DisabledMark).HasColumnName("Disabled_Mark");

                entity.Property(e => e.DrawName)
                    .HasMaxLength(50)
                    .HasColumnName("Draw_Name");

                entity.Property(e => e.DrawYear)
                    .HasMaxLength(10)
                    .HasColumnName("Draw_Year");

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .HasColumnName("Insert_Time");

                entity.Property(e => e.InsertUser)
                    .HasMaxLength(22)
                    .HasColumnName("Insert_User");

                entity.Property(e => e.ModifyTime)
                    .HasMaxLength(22)
                    .HasColumnName("Modify_Time");

                entity.Property(e => e.ModifyUser)
                    .HasMaxLength(22)
                    .HasColumnName("Modify_User");
            });

            modelBuilder.Entity<TbLuckydrawPrize>(entity =>
            {
                entity.HasKey(e => e.PrizeId)
                    .HasName("PK__TB_LUCKY__0E27005E2BC97F7C");

                entity.ToTable("TB_LUCKYDRAW_PRIZE");

                entity.Property(e => e.PrizeId).HasColumnName("Prize_ID");

                entity.Property(e => e.DisabledMark).HasColumnName("Disabled_Mark");

                entity.Property(e => e.DrawAmount)
                    .HasColumnName("Draw_Amount")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DrawId).HasColumnName("Draw_ID");

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .HasColumnName("Insert_Time");

                entity.Property(e => e.InsertUser)
                    .HasMaxLength(22)
                    .HasColumnName("Insert_User");

                entity.Property(e => e.ModifyTime)
                    .HasMaxLength(22)
                    .HasColumnName("Modify_Time");

                entity.Property(e => e.ModifyUser)
                    .HasMaxLength(22)
                    .HasColumnName("Modify_User");

                entity.Property(e => e.OverAyearMark).HasColumnName("OverAYear_Mark");

                entity.Property(e => e.PrizeAmount).HasColumnName("Prize_Amount");

                entity.Property(e => e.PrizeName)
                    .HasMaxLength(50)
                    .HasColumnName("Prize_Name");

                entity.Property(e => e.PrizePic)
                    .HasMaxLength(2000)
                    .HasColumnName("Prize_Pic");

                entity.Property(e => e.PrizePrice).HasColumnName("Prize_Price");

                entity.Property(e => e.SortNo).HasColumnName("Sort_No");
            });

            modelBuilder.Entity<TbLuckydrawPrizewinning>(entity =>
            {
                entity.HasKey(e => e.WinningId)
                    .HasName("PK__TB_LUCKY__D1539F293BFFE745");

                entity.ToTable("TB_LUCKYDRAW_PRIZEWINNING");

                entity.Property(e => e.WinningId).HasColumnName("Winning_ID");

                entity.Property(e => e.DisabledMark).HasColumnName("Disabled_Mark");

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .HasColumnName("Insert_Time");

                entity.Property(e => e.InsertUser)
                    .HasMaxLength(22)
                    .HasColumnName("Insert_User");

                entity.Property(e => e.ModifyTime)
                    .HasMaxLength(22)
                    .HasColumnName("Modify_Time");

                entity.Property(e => e.ModifyUser)
                    .HasMaxLength(22)
                    .HasColumnName("Modify_User");

                entity.Property(e => e.PrizeId).HasColumnName("Prize_ID");

                entity.Property(e => e.UserErpid)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("User_ERPID");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("User_Name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
