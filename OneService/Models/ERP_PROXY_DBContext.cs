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

        public virtual DbSet<CustomerContact> CustomerContacts { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<PostalaAddressAndCode> PostalaAddressAndCodes { get; set; } = null!;
        public virtual DbSet<Stockall> Stockalls { get; set; } = null!;
        public virtual DbSet<Stockwty> Stockwties { get; set; } = null!;
        public virtual DbSet<ViewCustomer2> ViewCustomer2s { get; set; } = null!;
        public virtual DbSet<ViewMaterialByComp> ViewMaterialByComps { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=TSTI-SAP-PROXY;Database=ERP_PROXY_DB;User=sa;Password=Eip@dmin");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerContact>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.ToTable("CUSTOMER_Contact");

                entity.HasIndex(e => new { e.Kna1Kunnr, e.Knb1Bukrs }, "NonClusteredIndex-20221004-134356");

                entity.Property(e => e.ContactId)
                    .HasColumnName("ContactID")
                    .HasDefaultValueSql("(newid())")
                    .HasComment("ID");

                entity.Property(e => e.BpmNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("BPM表單編號");

                entity.Property(e => e.ContactAddress)
                    .HasMaxLength(100)
                    .HasComment("聯絡人地址");

                entity.Property(e => e.ContactCity)
                    .HasMaxLength(10)
                    .HasComment("聯絡人縣市");

                entity.Property(e => e.ContactDepartment)
                    .HasMaxLength(100)
                    .HasComment("聯絡人部門");

                entity.Property(e => e.ContactEmail)
                    .HasMaxLength(200)
                    .HasComment("聯絡人Email");

                entity.Property(e => e.ContactName)
                    .HasMaxLength(40)
                    .HasComment("聯絡人姓名");

                entity.Property(e => e.ContactPhone)
                    .HasMaxLength(50)
                    .HasComment("聯絡人電話");

                entity.Property(e => e.ContactPosition)
                    .HasMaxLength(20)
                    .HasComment("聯絡人職稱");

                entity.Property(e => e.ContactType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasComment("聯絡人類別(0:發票,1:驗收,2:收貨,3:存出保證金經辦,4:客戶建檔聯絡人)");

                entity.Property(e => e.IsMain).HasComment("是否為對帳窗口");

                entity.Property(e => e.Kna1Kunnr)
                    .HasMaxLength(10)
                    .HasColumnName("KNA1_KUNNR")
                    .HasComment("客戶代號");

                entity.Property(e => e.Kna1Name1)
                    .HasMaxLength(35)
                    .HasColumnName("KNA1_NAME1")
                    .HasComment("客戶名稱");

                entity.Property(e => e.Knb1Bukrs)
                    .HasMaxLength(4)
                    .HasColumnName("KNB1_BUKRS")
                    .HasComment("公司代碼");

                entity.Property(e => e.MainModifiedDate)
                    .HasColumnType("datetime")
                    .HasComment("對帳窗口更新日期");

                entity.Property(e => e.MainModifiedUserId)
                    .HasColumnName("MainModifiedUserID")
                    .HasComment("對帳窗口更新者ID");

                entity.Property(e => e.MainModifiedUserName)
                    .HasMaxLength(50)
                    .HasComment("對帳窗口更新者姓名");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasComment("更新日期");

                entity.Property(e => e.ModifiedUserId)
                    .HasColumnName("ModifiedUserID")
                    .HasComment("更新者ID");

                entity.Property(e => e.ModifiedUserName)
                    .HasMaxLength(50)
                    .HasComment("更新者姓名");
            });

            modelBuilder.Entity<Material>(entity =>
            {
                entity.HasKey(e => new { e.MaraMatnr, e.MardWerks, e.MardLgort, e.MvkeVkorg, e.MvkeVtweg });

                entity.ToTable("MATERIAL");

                entity.Property(e => e.MaraMatnr)
                    .HasMaxLength(18)
                    .HasColumnName("MARA_MATNR")
                    .HasComment("物料號碼");

                entity.Property(e => e.MardWerks)
                    .HasMaxLength(4)
                    .HasColumnName("MARD_WERKS")
                    .HasComment("工廠");

                entity.Property(e => e.MardLgort)
                    .HasMaxLength(4)
                    .HasColumnName("MARD_LGORT")
                    .HasComment("儲存地點");

                entity.Property(e => e.MvkeVkorg)
                    .HasMaxLength(4)
                    .HasColumnName("MVKE_VKORG")
                    .HasComment("銷售組織");

                entity.Property(e => e.MvkeVtweg)
                    .HasMaxLength(2)
                    .HasColumnName("MVKE_VTWEG")
                    .HasComment("通路");

                entity.Property(e => e.BasicContent).HasComment("基本物料內文");

                entity.Property(e => e.MaktTxza1En)
                    .HasMaxLength(40)
                    .HasColumnName("MAKT_TXZA1_EN")
                    .HasComment("短文(英文-EN)");

                entity.Property(e => e.MaktTxza1Zf)
                    .HasMaxLength(40)
                    .HasColumnName("MAKT_TXZA1_ZF")
                    .HasComment("短文(中文-ZF)");

                entity.Property(e => e.MaraMatkl)
                    .HasMaxLength(9)
                    .HasColumnName("MARA_MATKL")
                    .HasComment("物料群組");

                entity.Property(e => e.MaraMfrpn)
                    .HasMaxLength(50)
                    .HasColumnName("MARA_MFRPN")
                    .HasComment("原廠料號欄位-製造商零件號碼");

                entity.Property(e => e.MaraStxl1)
                    .HasMaxLength(200)
                    .HasColumnName("MARA_STXL1")
                    .HasComment("保固條件(採購-中文)");

                entity.Property(e => e.MaraStxl2)
                    .HasMaxLength(200)
                    .HasColumnName("MARA_STXL2")
                    .HasComment("保固條件(銷售中文)");

                entity.Property(e => e.MaraStxl3)
                    .HasMaxLength(200)
                    .HasColumnName("MARA_STXL3")
                    .HasComment("保固條件(採購-英文)");

                entity.Property(e => e.MaraStxl4)
                    .HasMaxLength(200)
                    .HasColumnName("MARA_STXL4")
                    .HasComment("保固條件(銷售-英文)");

                entity.Property(e => e.MvkeProdh)
                    .HasMaxLength(18)
                    .HasColumnName("MVKE_PRODH")
                    .HasComment("產品階層");

                entity.Property(e => e.StorageBlock)
                    .HasMaxLength(20)
                    .HasComment("儲格");
            });

            modelBuilder.Entity<PostalaAddressAndCode>(entity =>
            {
                entity.HasKey(e => new { e.Code, e.City, e.Township, e.Road, e.No });

                entity.ToTable("PostalaAddressAndCode");

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.City).HasMaxLength(20);

                entity.Property(e => e.Township).HasMaxLength(20);

                entity.Property(e => e.Road).HasMaxLength(50);

                entity.Property(e => e.No).HasMaxLength(50);
            });

            modelBuilder.Entity<Stockall>(entity =>
            {
                entity.HasKey(e => e.IvSerial);

                entity.ToTable("STOCKALL");

                entity.HasIndex(e => new { e.VendoromSdate, e.VendoromEdate, e.OmSdate, e.OmEdate, e.ExSdate, e.ExEdate, e.TmSdate, e.TmEdate }, "NonClusteredIndex-20220901-160826");

                entity.Property(e => e.IvSerial)
                    .HasMaxLength(40)
                    .HasColumnName("IV_SERIAL");

                entity.Property(e => e.ContractEdate)
                    .HasColumnType("datetime")
                    .HasColumnName("ContractEDate");

                entity.Property(e => e.ContractId)
                    .HasMaxLength(10)
                    .HasColumnName("ContractID");

                entity.Property(e => e.ContractName).HasMaxLength(255);

                entity.Property(e => e.ContractSdate)
                    .HasColumnType("datetime")
                    .HasColumnName("ContractSDATE");

                entity.Property(e => e.CrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CR_DATE");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("CustomerID");

                entity.Property(e => e.CustomerName).HasMaxLength(35);

                entity.Property(e => e.ExEdate)
                    .HasColumnType("datetime")
                    .HasColumnName("EX_EDATE");

                entity.Property(e => e.ExSdate)
                    .HasColumnType("datetime")
                    .HasColumnName("EX_SDATE");

                entity.Property(e => e.ExWtyid)
                    .HasMaxLength(40)
                    .HasColumnName("EX_WTYID");

                entity.Property(e => e.Internalno)
                    .HasMaxLength(20)
                    .HasColumnName("INTERNALNO");

                entity.Property(e => e.IvDndate)
                    .HasColumnType("datetime")
                    .HasColumnName("IV_DNDATE");

                entity.Property(e => e.IvGrdate)
                    .HasColumnType("datetime")
                    .HasColumnName("IV_GRDATE");

                entity.Property(e => e.IvPono)
                    .HasMaxLength(10)
                    .HasColumnName("IV_PONO");

                entity.Property(e => e.IvSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("IV_SLARESP");

                entity.Property(e => e.IvSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("IV_SLASRV");

                entity.Property(e => e.IvVendor)
                    .HasMaxLength(10)
                    .HasColumnName("IV_VENDOR");

                entity.Property(e => e.IvVname)
                    .HasMaxLength(40)
                    .HasColumnName("IV_VNAME");

                entity.Property(e => e.OmEdate)
                    .HasColumnType("datetime")
                    .HasColumnName("OM_EDATE");

                entity.Property(e => e.OmSdate)
                    .HasColumnType("datetime")
                    .HasColumnName("OM_SDATE");

                entity.Property(e => e.OmWtyid)
                    .HasMaxLength(40)
                    .HasColumnName("OM_WTYID");

                entity.Property(e => e.OppNo).HasMaxLength(10);

                entity.Property(e => e.ProdHierarchy).HasMaxLength(18);

                entity.Property(e => e.ProdId)
                    .HasMaxLength(40)
                    .HasColumnName("ProdID");

                entity.Property(e => e.Product).HasMaxLength(50);

                entity.Property(e => e.ProjName).HasMaxLength(200);

                entity.Property(e => e.RenewStatus)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SoAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoNo).HasMaxLength(10);

                entity.Property(e => e.SoSales).HasMaxLength(20);

                entity.Property(e => e.SoSalesName).HasMaxLength(20);

                entity.Property(e => e.TmEdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TM_EDATE");

                entity.Property(e => e.TmSdate)
                    .HasColumnType("datetime")
                    .HasColumnName("TM_SDATE");

                entity.Property(e => e.TmWtyid)
                    .HasMaxLength(40)
                    .HasColumnName("TM_WTYID");

                entity.Property(e => e.VendoromEdate)
                    .HasColumnType("datetime")
                    .HasColumnName("VENDOROM_EDATE");

                entity.Property(e => e.VendoromSdate)
                    .HasColumnType("datetime")
                    .HasColumnName("VENDOROM_SDATE");

                entity.Property(e => e.VendoromWtyid)
                    .HasMaxLength(40)
                    .HasColumnName("VENDOROM_WTYID");
            });

            modelBuilder.Entity<Stockwty>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("STOCKWTY");

                entity.Property(e => e.CId)
                    .ValueGeneratedNever()
                    .HasColumnName("cID");

                entity.Property(e => e.Advice).HasColumnName("ADVICE");

                entity.Property(e => e.BpNo)
                    .HasMaxLength(40)
                    .HasColumnName("BP_NO");

                entity.Property(e => e.BpmNo)
                    .HasMaxLength(20)
                    .HasColumnName("BPM_NO");

                entity.Property(e => e.CarePackNo)
                    .HasMaxLength(50)
                    .HasColumnName("CARE_PACK_NO");

                entity.Property(e => e.CrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CR_DATE");

                entity.Property(e => e.CrUser)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CR_USER");

                entity.Property(e => e.IvCid)
                    .HasMaxLength(10)
                    .HasColumnName("IV_CID");

                entity.Property(e => e.IvDndate)
                    .HasColumnType("datetime")
                    .HasColumnName("IV_DNDATE");

                entity.Property(e => e.IvEdate)
                    .HasColumnType("datetime")
                    .HasColumnName("IV_EDATE");

                entity.Property(e => e.IvSdate)
                    .HasColumnType("datetime")
                    .HasColumnName("IV_SDATE");

                entity.Property(e => e.IvSerial)
                    .HasMaxLength(40)
                    .HasColumnName("IV_SERIAL");

                entity.Property(e => e.IvSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("IV_SLARESP");

                entity.Property(e => e.IvSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("IV_SLASRV");

                entity.Property(e => e.IvSono)
                    .HasMaxLength(10)
                    .HasColumnName("IV_SONO");

                entity.Property(e => e.IvWtydesc)
                    .HasMaxLength(50)
                    .HasColumnName("IV_WTYDESC");

                entity.Property(e => e.IvWtyid)
                    .HasMaxLength(40)
                    .HasColumnName("IV_WTYID");

                entity.Property(e => e.Note).HasColumnName("NOTE");

                entity.Property(e => e.ReceiptDate).HasMaxLength(22);

                entity.Property(e => e.ReceiptNo).HasMaxLength(100);

                entity.Property(e => e.UpDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UP_DATE");

                entity.Property(e => e.UpUser)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UP_USER");
            });

            modelBuilder.Entity<ViewCustomer2>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_CUSTOMER_2");

                entity.Property(e => e.Kna1Kunnr)
                    .HasMaxLength(10)
                    .HasColumnName("KNA1_KUNNR");

                entity.Property(e => e.Kna1Name1)
                    .HasMaxLength(35)
                    .HasColumnName("KNA1_NAME1");

                entity.Property(e => e.KnvvKdgrp)
                    .HasMaxLength(2)
                    .HasColumnName("KNVV_KDGRP");

                entity.Property(e => e.KnvvVkorg)
                    .HasMaxLength(4)
                    .HasColumnName("KNVV_VKORG");
            });

            modelBuilder.Entity<ViewMaterialByComp>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_MATERIAL_ByComp");

                entity.Property(e => e.MaktTxza1Zf)
                    .HasMaxLength(40)
                    .HasColumnName("MAKT_TXZA1_ZF");

                entity.Property(e => e.MaraMatkl)
                    .HasMaxLength(9)
                    .HasColumnName("MARA_MATKL");

                entity.Property(e => e.MaraMatnr)
                    .HasMaxLength(18)
                    .HasColumnName("MARA_MATNR");

                entity.Property(e => e.MaraStxl1)
                    .HasMaxLength(200)
                    .HasColumnName("MARA_STXL1");

                entity.Property(e => e.MardWerks)
                    .HasMaxLength(4)
                    .HasColumnName("MARD_WERKS");

                entity.Property(e => e.MvkeProdh)
                    .HasMaxLength(18)
                    .HasColumnName("MVKE_PRODH");

                entity.Property(e => e.MvkeVkorg)
                    .HasMaxLength(4)
                    .HasColumnName("MVKE_VKORG");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
