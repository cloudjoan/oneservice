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
        public virtual DbSet<CustomerContactStore> CustomerContactStores { get; set; } = null!;
        public virtual DbSet<F0005> F0005s { get; set; } = null!;
        public virtual DbSet<F4301codetail> F4301codetails { get; set; } = null!;
        public virtual DbSet<F4501> F4501s { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<PersonalContact> PersonalContacts { get; set; } = null!;
        public virtual DbSet<PostalaAddressAndCode> PostalaAddressAndCodes { get; set; } = null!;
        public virtual DbSet<So> Sos { get; set; } = null!;
        public virtual DbSet<Stockall> Stockalls { get; set; } = null!;
        public virtual DbSet<TbCrmOppHead> TbCrmOppHeads { get; set; } = null!;
        public virtual DbSet<TbMailContent> TbMailContents { get; set; } = null!;
        public virtual DbSet<TbSrReport> TbSrReports { get; set; } = null!;
        public virtual DbSet<ViewCustomer2> ViewCustomer2s { get; set; } = null!;
        public virtual DbSet<ViewCustomerandpersonal> ViewCustomerandpersonals { get; set; } = null!;
        public virtual DbSet<ViewMaterialByComp> ViewMaterialByComps { get; set; } = null!;

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

                entity.Property(e => e.ContactMobile)
                    .HasMaxLength(50)
                    .HasComment("聯絡人手機");

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
                    .HasComment("聯絡人類別(0:發票,1:驗收,2:收貨,3:存出保證金經辦,4:客戶建檔聯絡人,5.OneService建立)");

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

            modelBuilder.Entity<CustomerContactStore>(entity =>
            {
                entity.HasKey(e => e.ContactStoreId);

                entity.ToTable("CUSTOMER_ContactStore");

                entity.Property(e => e.ContactStoreId)
                    .ValueGeneratedNever()
                    .HasColumnName("ContactStoreID");

                entity.Property(e => e.ContactStoreName).HasMaxLength(40);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.Kna1Kunnr)
                    .HasMaxLength(10)
                    .HasColumnName("KNA1_KUNNR");

                entity.Property(e => e.Kna1Name1)
                    .HasMaxLength(35)
                    .HasColumnName("KNA1_NAME1");

                entity.Property(e => e.Knb1Bukrs)
                    .HasMaxLength(4)
                    .HasColumnName("KNB1_BUKRS");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<F0005>(entity =>
            {
                entity.HasKey(e => new { e.Modt, e.Alias, e.Codet, e.Codets });

                entity.ToTable("F0005");

                entity.Property(e => e.Modt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MODT");

                entity.Property(e => e.Alias)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ALIAS");

                entity.Property(e => e.Codet)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("CODET");

                entity.Property(e => e.Codets)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("CODETS");

                entity.Property(e => e.ApDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("AP_DATE");

                entity.Property(e => e.ApUser)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AP_USER");

                entity.Property(e => e.Coso).HasColumnName("COSO");

                entity.Property(e => e.CrDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("CR_DATE");

                entity.Property(e => e.CrUser)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CR_USER");

                entity.Property(e => e.Dsc1)
                    .HasMaxLength(80)
                    .HasColumnName("DSC1");

                entity.Property(e => e.Dsc2)
                    .HasMaxLength(80)
                    .HasColumnName("DSC2")
                    .HasComment("天數");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Re1)
                    .HasMaxLength(255)
                    .HasColumnName("RE1");

                entity.Property(e => e.Re2)
                    .HasMaxLength(255)
                    .HasColumnName("RE2");

                entity.Property(e => e.Re3).HasColumnName("RE3");

                entity.Property(e => e.Re4).HasColumnName("RE4");

                entity.Property(e => e.Re5)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("RE5");

                entity.Property(e => e.Re6)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("RE6");

                entity.Property(e => e.Rmk)
                    .HasMaxLength(80)
                    .HasColumnName("RMK");

                entity.Property(e => e.UpDate)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("UP_DATE");

                entity.Property(e => e.UpUser)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UP_USER");
            });

            modelBuilder.Entity<F4301codetail>(entity =>
            {
                entity.HasKey(e => new { e.CompanyId, e.InternalNo });

                entity.ToTable("F4301CODetail");

                entity.HasIndex(e => new { e.InternalNo, e.Applicant, e.OldInternalNo }, "NonClusteredIndex-20230110-140821");

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

            modelBuilder.Entity<F4501>(entity =>
            {
                entity.ToTable("F4501");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasComment("項次(系統自動產生)");

                entity.Property(e => e.Aaccount)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("AAccount")
                    .HasComment("申請人帳號");

                entity.Property(e => e.Acc)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ACC")
                    .HasComment("申請人成本中心");

                entity.Property(e => e.Adate)
                    .HasColumnType("datetime")
                    .HasColumnName("ADate")
                    .HasComment("申請日期");

                entity.Property(e => e.Adept)
                    .HasMaxLength(200)
                    .HasColumnName("ADept")
                    .HasComment("申請人單位");

                entity.Property(e => e.AdeptId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("ADeptID")
                    .HasComment("申請人單位ID");

                entity.Property(e => e.Aeid)
                    .HasColumnName("AEID")
                    .HasComment("申請人員工編號");

                entity.Property(e => e.Apc)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("APC")
                    .HasComment("申請人利潤中心");

                entity.Property(e => e.AppType)
                    .HasMaxLength(10)
                    .HasComment("審核方式(相關文件審核)");

                entity.Property(e => e.Asapid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ASAPID")
                    .HasComment("申請人員工ID(SAP)");

                entity.Property(e => e.Atitle)
                    .HasMaxLength(30)
                    .HasColumnName("ATitle")
                    .HasComment("申請人職稱");

                entity.Property(e => e.Awp)
                    .HasMaxLength(50)
                    .HasColumnName("AWP")
                    .HasComment("申請人工作地點");

                entity.Property(e => e.Bc)
                    .HasMaxLength(1000)
                    .HasColumnName("BC")
                    .HasComment("預期一天罰金為總金額之佔比");

                entity.Property(e => e.BillableTime)
                    .HasColumnName("BILLABLE_TIME")
                    .HasComment("請款周期");

                entity.Property(e => e.Bpmno)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BPMNo")
                    .HasComment("BPM表單編號");

                entity.Property(e => e.Bs)
                    .HasMaxLength(10)
                    .HasColumnName("BS")
                    .HasComment("公司大章");

                entity.Property(e => e.Camt)
                    .HasColumnName("CAmt")
                    .HasComment("合約金額");

                entity.Property(e => e.Ccontent)
                    .HasColumnName("CContent")
                    .HasComment("文件內容簡述(目的)");

                entity.Property(e => e.Cdate)
                    .HasColumnType("datetime")
                    .HasColumnName("CDate")
                    .HasComment("文件期間");

                entity.Property(e => e.Cdept)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("CDept")
                    .HasComment("文件保管單位");

                entity.Property(e => e.Classified)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("文件機密等級");

                entity.Property(e => e.CloseDate)
                    .HasColumnType("datetime")
                    .HasComment("結案日期");

                entity.Property(e => e.Cname)
                    .HasMaxLength(500)
                    .HasColumnName("CName")
                    .HasComment("文件名稱(案名)");

                entity.Property(e => e.CompCde)
                    .HasMaxLength(10)
                    .HasColumnName("Comp_Cde")
                    .HasDefaultValueSql("(N'Comp-1')")
                    .IsFixedLength()
                    .HasComment("公司別");

                entity.Property(e => e.CounterSign)
                    .HasMaxLength(100)
                    .HasComment("會簽單位");

                entity.Property(e => e.Cp)
                    .HasMaxLength(1000)
                    .HasColumnName("CP")
                    .HasComment("收付款條件");

                entity.Property(e => e.Cpic)
                    .HasMaxLength(30)
                    .HasColumnName("CPIC")
                    .HasComment("文件承辦人");

                entity.Property(e => e.CrDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CR_DATE")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("建檔日期/時間");

                entity.Property(e => e.CrUser)
                    .HasMaxLength(50)
                    .HasColumnName("CR_USER")
                    .HasComment("建檔者帳號/姓名");

                entity.Property(e => e.CrmOppNo)
                    .HasMaxLength(20)
                    .HasComment("商機號碼");

                entity.Property(e => e.Crno)
                    .HasMaxLength(50)
                    .HasColumnName("CRNo")
                    .HasComment("收發文字號");

                entity.Property(e => e.Cs)
                    .HasMaxLength(10)
                    .HasColumnName("CS")
                    .HasComment("投標專用章");

                entity.Property(e => e.Csd)
                    .HasMaxLength(1000)
                    .HasColumnName("CSD")
                    .HasComment("履約保證金");

                entity.Property(e => e.Ctype)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("CType")
                    .HasComment("文件類型");

                entity.Property(e => e.CtypeOther)
                    .HasMaxLength(500)
                    .HasComment("文件類型(其他說明)");

                entity.Property(e => e.Cvr)
                    .HasMaxLength(100)
                    .HasColumnName("CVR")
                    .HasComment("作廢原因");

                entity.Property(e => e.Datestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("DATESTAMP")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("最近異動日期/時間");

                entity.Property(e => e.Dp)
                    .HasMaxLength(1000)
                    .HasColumnName("DP")
                    .HasComment("糾紛處理");

                entity.Property(e => e.Dplace)
                    .HasMaxLength(1000)
                    .HasColumnName("DPlace")
                    .HasComment("糾紛處理地點");

                entity.Property(e => e.EcsapplyNo)
                    .HasMaxLength(10)
                    .HasColumnName("ECSApplyNo")
                    .HasComment("ECS申請號");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasComment("合約終止日期");

                entity.Property(e => e.ErpAssid)
                    .HasMaxLength(10)
                    .HasColumnName("ERP_ASSID")
                    .HasComment("業務祕書ERPID");

                entity.Property(e => e.Extend)
                    .HasColumnType("datetime")
                    .HasComment("到期自動延長日期");

                entity.Property(e => e.Faccount)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("FAccount")
                    .HasComment("填表人帳號");

                entity.Property(e => e.Fcc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FCC")
                    .HasComment("填表人成本中心");

                entity.Property(e => e.Fdept)
                    .HasMaxLength(200)
                    .HasColumnName("FDept")
                    .HasComment("填表人單位");

                entity.Property(e => e.FdeptId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("FDeptID")
                    .HasComment("填表人單位ID");

                entity.Property(e => e.Feid)
                    .HasColumnName("FEID")
                    .HasComment("填表人員工編號");

                entity.Property(e => e.First)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("文件速別");

                entity.Property(e => e.Flag)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasComment("流程狀態 - [空白]：正常，[S]：用印，[F]：結案，[I]：作廢");

                entity.Property(e => e.Fo)
                    .HasMaxLength(1000)
                    .HasColumnName("FO")
                    .HasComment("設備標的");

                entity.Property(e => e.Fpc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FPC")
                    .HasComment("填表人利潤中心");

                entity.Property(e => e.Fsapid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FSAPID")
                    .HasComment("填表人員工ID");

                entity.Property(e => e.Ftitle)
                    .HasMaxLength(30)
                    .HasColumnName("FTitle")
                    .HasComment("填表人職稱");

                entity.Property(e => e.Fwp)
                    .HasMaxLength(50)
                    .HasColumnName("FWP")
                    .HasComment("填表人工作地點");

                entity.Property(e => e.Idcard)
                    .HasMaxLength(10)
                    .HasColumnName("IDCard")
                    .HasComment("負責人身份證影本");

                entity.Property(e => e.Nature)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasComment("文件性質");

                entity.Property(e => e.No)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("NO")
                    .HasComment("文件編號");

                entity.Property(e => e.Ocpic)
                    .HasMaxLength(1000)
                    .HasColumnName("OCPIC");

                entity.Property(e => e.Os)
                    .HasMaxLength(10)
                    .HasColumnName("OS")
                    .HasComment("其他章");

                entity.Property(e => e.Pts)
                    .HasMaxLength(10)
                    .HasComment("份數");

                entity.Property(e => e.Remark).HasComment("備註");

                entity.Property(e => e.Sales)
                    .HasMaxLength(30)
                    .HasComment("文件業務代表");

                entity.Property(e => e.Sign)
                    .HasMaxLength(10)
                    .HasComment("簽名");

                entity.Property(e => e.Sp)
                    .HasMaxLength(10)
                    .HasColumnName("SP")
                    .HasComment("騎縫章");

                entity.Property(e => e.Ss)
                    .HasMaxLength(10)
                    .HasColumnName("SS")
                    .HasComment("小章");

                entity.Property(e => e.StrDate)
                    .HasColumnType("datetime")
                    .HasComment("合約開始日期");

                entity.Property(e => e.Target)
                    .HasMaxLength(500)
                    .HasComment("文件對象");

                entity.Property(e => e.TaxAmt).HasComment("印花稅總額");

                entity.Property(e => e.UploadFiles).HasMaxLength(1000);

                entity.Property(e => e.Userstamp)
                    .HasMaxLength(50)
                    .HasColumnName("USERSTAMP")
                    .HasComment("最近異動者帳號/姓名");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(300)
                    .HasComment("簽約對象身分");

                entity.Property(e => e.Wp)
                    .HasMaxLength(255)
                    .HasColumnName("WP")
                    .HasComment("保固期間");

                entity.Property(e => e.Year)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasComment("年度");
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

            modelBuilder.Entity<PersonalContact>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.ToTable("PERSONAL_Contact");

                entity.Property(e => e.ContactId)
                    .ValueGeneratedNever()
                    .HasColumnName("ContactID");

                entity.Property(e => e.ContactAddress).HasMaxLength(100);

                entity.Property(e => e.ContactCity).HasMaxLength(10);

                entity.Property(e => e.ContactEmail).HasMaxLength(200);

                entity.Property(e => e.ContactMobile).HasMaxLength(50);

                entity.Property(e => e.ContactName).HasMaxLength(40);

                entity.Property(e => e.ContactPhone).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.Kna1Kunnr)
                    .HasMaxLength(10)
                    .HasColumnName("KNA1_KUNNR");

                entity.Property(e => e.Kna1Name1)
                    .HasMaxLength(35)
                    .HasColumnName("KNA1_NAME1");

                entity.Property(e => e.Knb1Bukrs)
                    .HasMaxLength(4)
                    .HasColumnName("KNB1_BUKRS");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
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

            modelBuilder.Entity<So>(entity =>
            {
                entity.HasKey(e => new { e.Auart, e.Vbeln, e.Posnr, e.Vbeln1, e.Posnn })
                    .HasName("PK_SO_1");

                entity.ToTable("SO");

                entity.HasIndex(e => new { e.Bstnk, e.Status }, "NonClusteredIndex-20220826-192157");

                entity.Property(e => e.Auart)
                    .HasMaxLength(4)
                    .HasColumnName("AUART")
                    .HasComment("銷售文件類型");

                entity.Property(e => e.Vbeln)
                    .HasMaxLength(10)
                    .HasColumnName("VBELN")
                    .HasComment("銷售文件");

                entity.Property(e => e.Posnr)
                    .HasColumnType("numeric(6, 0)")
                    .HasColumnName("POSNR")
                    .HasComment("銷售文件項目");

                entity.Property(e => e.Vbeln1)
                    .HasMaxLength(10)
                    .HasColumnName("VBELN1")
                    .HasComment("出貨單號");

                entity.Property(e => e.Posnn)
                    .HasColumnType("numeric(6, 0)")
                    .HasColumnName("POSNN")
                    .HasComment("項目");

                entity.Property(e => e.Abgru)
                    .HasMaxLength(2)
                    .HasColumnName("ABGRU");

                entity.Property(e => e.Aedat)
                    .HasColumnType("datetime")
                    .HasColumnName("AEDAT")
                    .HasComment("更改日期");

                entity.Property(e => e.Aedat1)
                    .HasColumnType("datetime")
                    .HasColumnName("AEDAT1")
                    .HasComment("輸入時間");

                entity.Property(e => e.Arktx)
                    .HasMaxLength(50)
                    .HasColumnName("ARKTX")
                    .HasComment("說明");

                entity.Property(e => e.Audat)
                    .HasColumnType("datetime")
                    .HasColumnName("AUDAT")
                    .HasComment("文件日期");

                entity.Property(e => e.BstkdE)
                    .HasMaxLength(35)
                    .HasColumnName("BSTKD_E");

                entity.Property(e => e.Bstnk)
                    .HasMaxLength(20)
                    .HasColumnName("BSTNK")
                    .HasComment("採購單號碼");

                entity.Property(e => e.City1)
                    .HasMaxLength(40)
                    .HasColumnName("CITY1")
                    .HasComment("城市");

                entity.Property(e => e.City2)
                    .HasMaxLength(40)
                    .HasColumnName("CITY2")
                    .HasComment("城市");

                entity.Property(e => e.Country1)
                    .HasMaxLength(3)
                    .HasColumnName("COUNTRY1")
                    .HasComment("國家碼");

                entity.Property(e => e.Country2)
                    .HasMaxLength(3)
                    .HasColumnName("COUNTRY2")
                    .HasComment("國家碼");

                entity.Property(e => e.Erdat)
                    .HasColumnType("datetime")
                    .HasColumnName("ERDAT")
                    .HasComment("建立日期");

                entity.Property(e => e.Ernam)
                    .HasMaxLength(12)
                    .HasColumnName("ERNAM")
                    .HasComment("建立者");

                entity.Property(e => e.IndustryApply)
                    .HasMaxLength(100)
                    .HasColumnName("INDUSTRY_APPLY");

                entity.Property(e => e.InvoiceQuantity).HasColumnType("decimal(15, 3)");

                entity.Property(e => e.Kkber)
                    .HasMaxLength(4)
                    .HasColumnName("KKBER")
                    .HasComment("信用控制範圍");

                entity.Property(e => e.Kunnr)
                    .HasMaxLength(10)
                    .HasColumnName("KUNNR")
                    .HasComment("買方");

                entity.Property(e => e.Kunnr1)
                    .HasMaxLength(10)
                    .HasColumnName("KUNNR1")
                    .HasComment("收貨人");

                entity.Property(e => e.Kwmeng)
                    .HasColumnType("decimal(15, 3)")
                    .HasColumnName("KWMENG")
                    .HasComment("訂購數量");

                entity.Property(e => e.Lgmng)
                    .HasColumnType("decimal(15, 3)")
                    .HasColumnName("LGMNG")
                    .HasComment("交貨數量");

                entity.Property(e => e.Lgort)
                    .HasMaxLength(4)
                    .HasColumnName("LGORT")
                    .HasComment("儲存地點");

                entity.Property(e => e.Matnr)
                    .HasMaxLength(18)
                    .HasColumnName("MATNR")
                    .HasComment("物料");

                entity.Property(e => e.Mbdat)
                    .HasColumnType("datetime")
                    .HasColumnName("MBDAT")
                    .HasComment("供貨日期");

                entity.Property(e => e.Meins)
                    .HasMaxLength(3)
                    .HasColumnName("MEINS")
                    .HasComment("基礎計量單位");

                entity.Property(e => e.Nachn)
                    .HasMaxLength(40)
                    .HasColumnName("NACHN")
                    .HasComment("姓");

                entity.Property(e => e.Name1)
                    .HasMaxLength(40)
                    .HasColumnName("NAME1")
                    .HasComment("名稱");

                entity.Property(e => e.Name2)
                    .HasMaxLength(40)
                    .HasColumnName("NAME2")
                    .HasComment("名稱");

                entity.Property(e => e.Netpr)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NETPR");

                entity.Property(e => e.OldProfitCenterId)
                    .HasMaxLength(10)
                    .HasColumnName("OldProfitCenterID");

                entity.Property(e => e.PaymentTerms).HasMaxLength(50);

                entity.Property(e => e.Pernr)
                    .HasColumnType("numeric(8, 0)")
                    .HasColumnName("PERNR")
                    .HasComment("員工號碼");

                entity.Property(e => e.PinvoiceDate)
                    .HasColumnType("datetime")
                    .HasColumnName("PInvoiceDate");

                entity.Property(e => e.PostCode1)
                    .HasMaxLength(10)
                    .HasColumnName("POST_CODE1")
                    .HasComment("郵遞區號");

                entity.Property(e => e.PostCode2)
                    .HasMaxLength(10)
                    .HasColumnName("POST_CODE2")
                    .HasComment("郵遞區號");

                entity.Property(e => e.Preservation)
                    .HasMaxLength(40)
                    .HasColumnName("PRESERVATION")
                    .HasComment("保固條件");

                entity.Property(e => e.ProfitCenterId)
                    .HasMaxLength(10)
                    .HasColumnName("ProfitCenterID");

                entity.Property(e => e.PurchaseAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PurchaseWaers)
                    .HasMaxLength(5)
                    .HasColumnName("PurchaseWAERS");

                entity.Property(e => e.ReceiverName2).HasMaxLength(500);

                entity.Property(e => e.ReceiverName3).HasMaxLength(500);

                entity.Property(e => e.ReceiverName4).HasMaxLength(500);

                entity.Property(e => e.Spart)
                    .HasMaxLength(2)
                    .HasColumnName("SPART")
                    .HasComment("部門");

                entity.Property(e => e.Status).HasMaxLength(2);

                entity.Property(e => e.Street1)
                    .HasMaxLength(60)
                    .HasColumnName("STREET1")
                    .HasComment("街道");

                entity.Property(e => e.Street2)
                    .HasMaxLength(60)
                    .HasColumnName("STREET2")
                    .HasComment("街道");

                entity.Property(e => e.Vdatu)
                    .HasColumnType("datetime")
                    .HasColumnName("VDATU")
                    .HasComment("請求交貨日期");

                entity.Property(e => e.Vkbur)
                    .HasMaxLength(4)
                    .HasColumnName("VKBUR")
                    .HasComment("銷售據點");

                entity.Property(e => e.Vkgrp)
                    .HasMaxLength(3)
                    .HasColumnName("VKGRP")
                    .HasComment("銷售群組");

                entity.Property(e => e.Vkorg)
                    .HasMaxLength(4)
                    .HasColumnName("VKORG")
                    .HasComment("銷售組織");

                entity.Property(e => e.Vorna)
                    .HasMaxLength(40)
                    .HasColumnName("VORNA")
                    .HasComment("名");

                entity.Property(e => e.Vstel)
                    .HasMaxLength(4)
                    .HasColumnName("VSTEL")
                    .HasComment("出貨點/收貨點");

                entity.Property(e => e.Vtweg)
                    .HasMaxLength(2)
                    .HasColumnName("VTWEG")
                    .HasComment("配銷通路");

                entity.Property(e => e.Vvc01)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VVC01");

                entity.Property(e => e.Vvq01)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VVQ01");

                entity.Property(e => e.Vvr01)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VVR01");

                entity.Property(e => e.Waerk)
                    .HasMaxLength(5)
                    .HasColumnName("WAERK");

                entity.Property(e => e.Werks)
                    .HasMaxLength(4)
                    .HasColumnName("WERKS")
                    .HasComment("工廠");
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

                entity.Property(e => e.TechCheckStatus)
                    .HasMaxLength(2)
                    .HasColumnName("TechCheck_Status")
                    .HasDefaultValueSql("('0')");
            });

            modelBuilder.Entity<TbMailContent>(entity =>
            {
                entity.ToTable("TB_MAIL_CONTENT");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MailContent)
                    .HasColumnName("MAIL_CONTENT")
                    .HasComment("郵件內容");

                entity.Property(e => e.MailType)
                    .HasMaxLength(50)
                    .HasColumnName("MAIL_TYPE")
                    .HasComment("郵件類型");
            });

            modelBuilder.Entity<TbSrReport>(entity =>
            {
                entity.ToTable("TB_SR_REPORT");

                entity.HasComment("服務請求總表");

                entity.HasIndex(e => new { e.ObjectId, e.Customerid, e.Contactid, e.Sn, e.Csrid }, "NonClusteredIndex-20221024-102229");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(60)
                    .HasColumnName("ADDRESS")
                    .HasComment("地址");

                entity.Property(e => e.Arrive)
                    .HasMaxLength(20)
                    .HasColumnName("ARRIVE")
                    .HasComment("到場");

                entity.Property(e => e.Backupsn)
                    .HasMaxLength(300)
                    .HasColumnName("BACKUPSN");

                entity.Property(e => e.Changepart)
                    .HasMaxLength(1000)
                    .HasColumnName("CHANGEPART");

                entity.Property(e => e.Contact)
                    .HasMaxLength(40)
                    .HasColumnName("CONTACT")
                    .HasComment("聯絡人");

                entity.Property(e => e.Contactid)
                    .HasMaxLength(10)
                    .HasColumnName("CONTACTID")
                    .HasComment("聯絡人ID");

                entity.Property(e => e.Contract)
                    .HasMaxLength(20)
                    .HasColumnName("CONTRACT")
                    .HasComment("合約編號");

                entity.Property(e => e.Countin)
                    .HasMaxLength(20)
                    .HasColumnName("COUNTIN")
                    .HasComment("計數器(IN)");

                entity.Property(e => e.Countout)
                    .HasMaxLength(20)
                    .HasColumnName("COUNTOUT")
                    .HasComment("計數器(OUT)");

                entity.Property(e => e.Create)
                    .HasMaxLength(20)
                    .HasColumnName("CREATE_");

                entity.Property(e => e.Csrid)
                    .HasMaxLength(40)
                    .HasColumnName("CSRID")
                    .HasComment("服務報告ID");

                entity.Property(e => e.Csrlink)
                    .HasMaxLength(100)
                    .HasColumnName("CSRLINK")
                    .HasComment("服務報告書連結");

                entity.Property(e => e.Customer)
                    .HasMaxLength(40)
                    .HasColumnName("CUSTOMER")
                    .HasComment("客戶名稱");

                entity.Property(e => e.Customerid)
                    .HasMaxLength(10)
                    .HasColumnName("CUSTOMERID")
                    .HasComment("客戶ID");

                entity.Property(e => e.Dealway)
                    .HasMaxLength(20)
                    .HasColumnName("DEALWAY")
                    .HasComment("處理方式");

                entity.Property(e => e.Depart)
                    .HasMaxLength(20)
                    .HasColumnName("DEPART");

                entity.Property(e => e.Description)
                    .HasMaxLength(40)
                    .HasColumnName("DESCRIPTION")
                    .HasComment("說明");

                entity.Property(e => e.Dn)
                    .HasMaxLength(20)
                    .HasColumnName("DN")
                    .HasComment("出貨單號");

                entity.Property(e => e.Email)
                    .HasMaxLength(250)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Engineer)
                    .HasMaxLength(40)
                    .HasColumnName("ENGINEER")
                    .HasComment("工程師姓名");

                entity.Property(e => e.Engineerid)
                    .HasMaxLength(20)
                    .HasColumnName("ENGINEERID")
                    .HasComment("工程師ID");

                entity.Property(e => e.Finish)
                    .HasMaxLength(20)
                    .HasColumnName("FINISH")
                    .HasComment("服務結束");

                entity.Property(e => e.Hardware)
                    .HasMaxLength(2)
                    .HasColumnName("HARDWARE");

                entity.Property(e => e.Hpct)
                    .HasMaxLength(1000)
                    .HasColumnName("HPCT");

                entity.Property(e => e.Hpxc)
                    .HasMaxLength(300)
                    .HasColumnName("HPXC");

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .HasColumnName("INSERT_TIME")
                    .HasComment("建立時間");

                entity.Property(e => e.Labor)
                    .HasMaxLength(20)
                    .HasColumnName("LABOR");

                entity.Property(e => e.Mobile)
                    .HasMaxLength(30)
                    .HasColumnName("MOBILE")
                    .HasComment("手機");

                entity.Property(e => e.Newct)
                    .HasMaxLength(1000)
                    .HasColumnName("NEWCT");

                entity.Property(e => e.Oa)
                    .HasMaxLength(20)
                    .HasColumnName("OA");

                entity.Property(e => e.ObjectId)
                    .HasMaxLength(10)
                    .HasColumnName("OBJECT_ID")
                    .HasComment("服務請求id");

                entity.Property(e => e.Oldct)
                    .HasMaxLength(1000)
                    .HasColumnName("OLDCT");

                entity.Property(e => e.Other)
                    .HasMaxLength(2)
                    .HasColumnName("OTHER");

                entity.Property(e => e.Pid)
                    .HasMaxLength(40)
                    .HasColumnName("PID");

                entity.Property(e => e.Pn)
                    .HasMaxLength(40)
                    .HasColumnName("PN");

                entity.Property(e => e.Problem)
                    .HasMaxLength(2048)
                    .HasColumnName("PROBLEM");

                entity.Property(e => e.ProcessType)
                    .HasMaxLength(4)
                    .HasColumnName("PROCESS_TYPE")
                    .HasComment("類型");

                entity.Property(e => e.ProcessTypeText)
                    .HasMaxLength(40)
                    .HasColumnName("PROCESS_TYPE_TEXT")
                    .HasComment("類型說明");

                entity.Property(e => e.Reason)
                    .HasMaxLength(60)
                    .HasColumnName("REASON")
                    .HasComment("延遲原因");

                entity.Property(e => e.Refix)
                    .HasMaxLength(2)
                    .HasColumnName("REFIX")
                    .HasComment("二修");

                entity.Property(e => e.Replace)
                    .HasMaxLength(2)
                    .HasColumnName("REPLACE");

                entity.Property(e => e.Reset)
                    .HasMaxLength(2)
                    .HasColumnName("RESET");

                entity.Property(e => e.Restore)
                    .HasMaxLength(2)
                    .HasColumnName("RESTORE_");

                entity.Property(e => e.Rkind1)
                    .HasMaxLength(40)
                    .HasColumnName("RKIND1")
                    .HasComment("報修類別");

                entity.Property(e => e.Rkind2)
                    .HasMaxLength(40)
                    .HasColumnName("RKIND2")
                    .HasComment("報修大類");

                entity.Property(e => e.Rkind3)
                    .HasMaxLength(40)
                    .HasColumnName("RKIND3")
                    .HasComment("報修代碼");

                entity.Property(e => e.Sales)
                    .HasMaxLength(40)
                    .HasColumnName("SALES")
                    .HasComment("業務例外");

                entity.Property(e => e.Shop)
                    .HasMaxLength(40)
                    .HasColumnName("SHOP");

                entity.Property(e => e.Slaresp)
                    .HasMaxLength(20)
                    .HasColumnName("SLARESP")
                    .HasComment("回應條件");

                entity.Property(e => e.Slasrv)
                    .HasMaxLength(20)
                    .HasColumnName("SLASRV")
                    .HasComment("服務條件");

                entity.Property(e => e.Sn)
                    .HasMaxLength(40)
                    .HasColumnName("SN");

                entity.Property(e => e.So)
                    .HasMaxLength(20)
                    .HasColumnName("SO")
                    .HasComment("銷售單號");

                entity.Property(e => e.Software)
                    .HasMaxLength(2)
                    .HasColumnName("SOFTWARE");

                entity.Property(e => e.Solution)
                    .HasMaxLength(2048)
                    .HasColumnName("SOLUTION")
                    .HasComment("SOLUTION");

                entity.Property(e => e.Sq)
                    .HasMaxLength(40)
                    .HasColumnName("SQ");

                entity.Property(e => e.Srvkind)
                    .HasMaxLength(20)
                    .HasColumnName("SRVKIND")
                    .HasComment("維護服務種類");

                entity.Property(e => e.Srvteam)
                    .HasMaxLength(50)
                    .HasColumnName("SRVTEAM")
                    .HasComment("服務團隊");

                entity.Property(e => e.Status)
                    .HasMaxLength(5)
                    .HasColumnName("STATUS")
                    .HasComment("狀態");

                entity.Property(e => e.StatusText)
                    .HasMaxLength(40)
                    .HasColumnName("STATUS_TEXT")
                    .HasComment("狀態說明");

                entity.Property(e => e.Tel)
                    .HasMaxLength(30)
                    .HasColumnName("TEL")
                    .HasComment("電話");

                entity.Property(e => e.Unit)
                    .HasMaxLength(20)
                    .HasColumnName("UNIT")
                    .HasComment("維護服務單位");

                entity.Property(e => e.WtyEnd)
                    .HasMaxLength(20)
                    .HasColumnName("WTY_END")
                    .HasComment("保固結束");

                entity.Property(e => e.WtyStart)
                    .HasMaxLength(20)
                    .HasColumnName("WTY_START")
                    .HasComment("保固開始");

                entity.Property(e => e.Wtydesc)
                    .HasMaxLength(40)
                    .HasColumnName("WTYDESC")
                    .HasComment("保固描述");

                entity.Property(e => e.Wtyid)
                    .HasMaxLength(40)
                    .HasColumnName("WTYID");
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

            modelBuilder.Entity<ViewCustomerandpersonal>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_CUSTOMERANDPERSONAL");

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
