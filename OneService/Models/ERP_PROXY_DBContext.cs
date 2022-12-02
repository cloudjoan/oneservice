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

        public virtual DbSet<TbCrmOppHead> TbCrmOppHeads { get; set; } = null!;

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
            modelBuilder.Entity<TbCrmOppHead>(entity =>
            {
                entity.ToTable("TB_CRM_OPP_HEAD");

                entity.HasComment("商機表頭檔");

                entity.HasIndex(e => new { e.CrmOppNo, e.CreateAccount }, "NonClusteredIndex-20220901-123346");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.CancelReason)
                    .HasMaxLength(120)
                    .IsUnicode(false)
                    .HasColumnName("CANCEL_REASON");

                entity.Property(e => e.ChangeTime)
                    .HasMaxLength(22)
                    .IsUnicode(false)
                    .HasColumnName("CHANGE_TIME");

                entity.Property(e => e.CompName)
                    .HasMaxLength(200)
                    .HasColumnName("COMP_NAME")
                    .HasComment("客戶名稱");

                entity.Property(e => e.ContractType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CONTRACT_TYPE");

                entity.Property(e => e.CreateAccount)
                    .HasMaxLength(15)
                    .HasColumnName("CREATE_ACCOUNT")
                    .HasComment("負責員工員編");

                entity.Property(e => e.CrmOppNo)
                    .HasMaxLength(10)
                    .HasColumnName("CRM_OPP_NO")
                    .HasComment("商機號碼");

                entity.Property(e => e.CurrPhase)
                    .HasMaxLength(10)
                    .HasColumnName("CURR_PHASE")
                    .HasComment("階段");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .HasColumnName("CURRENCY")
                    .HasComment("幣別");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("CUSTOMER_ID")
                    .HasComment("客戶ID");

                entity.Property(e => e.DisChannel)
                    .HasMaxLength(2)
                    .HasColumnName("DIS_CHANNEL");

                entity.Property(e => e.Division)
                    .HasMaxLength(2)
                    .HasColumnName("DIVISION");

                entity.Property(e => e.EstimateDate)
                    .HasMaxLength(22)
                    .HasColumnName("ESTIMATE_DATE")
                    .HasComment("預計結束日期");

                entity.Property(e => e.ExpEndDate)
                    .HasMaxLength(22)
                    .HasColumnName("EXP_END_DATE")
                    .HasComment("結束日期");

                entity.Property(e => e.ExpGrossprofit)
                    .HasMaxLength(22)
                    .HasColumnName("EXP_GROSSPROFIT")
                    .HasComment("預計毛利");

                entity.Property(e => e.ExpRevenue)
                    .HasMaxLength(20)
                    .HasColumnName("EXP_REVENUE")
                    .HasComment("預計營收");

                entity.Property(e => e.IndustryApply)
                    .HasMaxLength(100)
                    .HasColumnName("INDUSTRY_APPLY");

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .HasColumnName("INSERT_TIME")
                    .HasComment("建立時間");

                entity.Property(e => e.InvoiceDate)
                    .HasMaxLength(22)
                    .HasColumnName("INVOICE_DATE")
                    .HasComment("發票日期");

                entity.Property(e => e.OppDescription)
                    .HasMaxLength(200)
                    .HasColumnName("OPP_DESCRIPTION")
                    .HasComment("商機標題");

                entity.Property(e => e.OrderType)
                    .HasMaxLength(10)
                    .HasColumnName("ORDER_TYPE");

                entity.Property(e => e.OrgResp)
                    .HasMaxLength(12)
                    .HasColumnName("ORG_RESP");

                entity.Property(e => e.OrgShort)
                    .HasMaxLength(12)
                    .HasColumnName("ORG_SHORT");

                entity.Property(e => e.ProStage)
                    .HasMaxLength(2)
                    .HasColumnName("PRO_STAGE");

                entity.Property(e => e.SalesCycle)
                    .HasMaxLength(10)
                    .HasColumnName("SALES_CYCLE")
                    .HasComment("銷售類型");

                entity.Property(e => e.SalesGroup)
                    .HasMaxLength(12)
                    .HasColumnName("SALES_GROUP");

                entity.Property(e => e.SalesOffice)
                    .HasMaxLength(12)
                    .HasColumnName("SALES_OFFICE");

                entity.Property(e => e.StartDate)
                    .HasMaxLength(22)
                    .HasColumnName("START_DATE")
                    .HasComment("開始日期");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasColumnName("STATUS")
                    .HasComment("狀態");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
