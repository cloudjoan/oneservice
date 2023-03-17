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
        public virtual DbSet<F0005> F0005s { get; set; } = null!;
        public virtual DbSet<F4501> F4501s { get; set; } = null!;
        public virtual DbSet<Material> Materials { get; set; } = null!;
        public virtual DbSet<PersonalContact> PersonalContacts { get; set; } = null!;
        public virtual DbSet<PostalaAddressAndCode> PostalaAddressAndCodes { get; set; } = null!;
        public virtual DbSet<Stockall> Stockalls { get; set; } = null!;
        public virtual DbSet<TbMailContent> TbMailContents { get; set; } = null!;
        public virtual DbSet<ViewCustomer2> ViewCustomer2s { get; set; } = null!;
        public virtual DbSet<ViewCustomerandpersonal> ViewCustomerandpersonals { get; set; } = null!;
        public virtual DbSet<ViewMaterialByComp> ViewMaterialByComps { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.119;Database=ERP_PROXY_DB;User=sa;Password=Eip@dmin");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerContact>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.ToTable("CUSTOMER_Contact");

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

                entity.Property(e => e.ContactMobile).HasMaxLength(50);

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
                    .HasColumnName("DSC2");

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

            modelBuilder.Entity<Stockall>(entity =>
            {
                entity.HasKey(e => e.IvSerial);

                entity.ToTable("STOCKALL");

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
