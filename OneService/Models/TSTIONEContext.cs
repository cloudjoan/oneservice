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

        public virtual DbSet<TbOneContractDetailEng> TbOneContractDetailEngs { get; set; } = null!;
        public virtual DbSet<TbOneContractDetailObj> TbOneContractDetailObjs { get; set; } = null!;
        public virtual DbSet<TbOneContractDetailSub> TbOneContractDetailSubs { get; set; } = null!;
        public virtual DbSet<TbOneContractMain> TbOneContractMains { get; set; } = null!;
        public virtual DbSet<TbOneDocument> TbOneDocuments { get; set; } = null!;
        public virtual DbSet<TbOneLog> TbOneLogs { get; set; } = null!;
        public virtual DbSet<TbOneSrcustomerEmailMapping> TbOneSrcustomerEmailMappings { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailContact> TbOneSrdetailContacts { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailMaterialInfo> TbOneSrdetailMaterialInfos { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailPartsReplace> TbOneSrdetailPartsReplaces { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailProduct> TbOneSrdetailProducts { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailRecord> TbOneSrdetailRecords { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailSerialFeedback> TbOneSrdetailSerialFeedbacks { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailWarranty> TbOneSrdetailWarranties { get; set; } = null!;
        public virtual DbSet<TbOneSridformat> TbOneSridformats { get; set; } = null!;
        public virtual DbSet<TbOneSrmain> TbOneSrmains { get; set; } = null!;
        public virtual DbSet<TbOneSrrepairType> TbOneSrrepairTypes { get; set; } = null!;
        public virtual DbSet<TbOneSrsqperson> TbOneSrsqpeople { get; set; } = null!;
        public virtual DbSet<TbOneSrteamMapping> TbOneSrteamMappings { get; set; } = null!;
        public virtual DbSet<ViewOneSrreport> ViewOneSrreports { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.67;Database=TSTI-ONE;User=psip;Password=einck!@!NNd");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbOneContractDetailEng>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_ContractDetail_ENG");

                entity.HasIndex(e => new { e.CContractId, e.CContactStoreId }, "NonClusteredIndex-20230517-113504");

                entity.Property(e => e.CId)
                    .HasColumnName("cID")
                    .HasComment("系統ID");

                entity.Property(e => e.CContactStoreId)
                    .HasColumnName("cContactStoreID")
                    .HasComment("客戶聯絡人門市代號");

                entity.Property(e => e.CContactStoreName)
                    .HasMaxLength(40)
                    .HasColumnName("cContactStoreName")
                    .HasComment("客戶聯絡人門市名稱");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID")
                    .HasComment("文件編號");

                entity.Property(e => e.CEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cEngineerID")
                    .HasComment("工程師ERPID");

                entity.Property(e => e.CEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cEngineerName")
                    .HasComment("工程師姓名(中文+ 英文)");

                entity.Property(e => e.CIsMainEngineer)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsMainEngineer")
                    .HasComment("是否為主要工程師(Y、空白)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.Disabled).HasComment("是否停用(0.啟用 1.停用)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneContractDetailObj>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_ContractDetail_OBJ");

                entity.HasIndex(e => e.CContractId, "NonClusteredIndex-20230517-113539");

                entity.Property(e => e.CId)
                    .HasColumnName("cID")
                    .HasComment("系統ID");

                entity.Property(e => e.CAddress)
                    .HasMaxLength(100)
                    .HasColumnName("cAddress")
                    .HasComment("地點");

                entity.Property(e => e.CArea)
                    .HasMaxLength(30)
                    .HasColumnName("cArea")
                    .HasComment("區域");

                entity.Property(e => e.CBrands)
                    .HasMaxLength(30)
                    .HasColumnName("cBrands")
                    .HasComment("廠牌");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID")
                    .HasComment("文件編號");

                entity.Property(e => e.CHostName)
                    .HasMaxLength(120)
                    .HasColumnName("cHostName")
                    .HasComment("主機名稱");

                entity.Property(e => e.CLocation)
                    .HasMaxLength(100)
                    .HasColumnName("cLocation")
                    .HasComment("Location");

                entity.Property(e => e.CModel)
                    .HasMaxLength(40)
                    .HasColumnName("cModel")
                    .HasComment("產品模組");

                entity.Property(e => e.CNotes)
                    .HasMaxLength(255)
                    .HasColumnName("cNotes")
                    .HasComment("備註");

                entity.Property(e => e.CPid)
                    .HasMaxLength(40)
                    .HasColumnName("cPID")
                    .HasComment("PID");

                entity.Property(e => e.CSerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cSerialID")
                    .HasComment("序號");

                entity.Property(e => e.CSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("cSLARESP")
                    .HasComment("SLA回應條件");

                entity.Property(e => e.CSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("cSLASRV")
                    .HasComment("SLA服務條件");

                entity.Property(e => e.CSubContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cSubContractID")
                    .HasComment("下包文件編號");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.Disabled).HasComment("是否停用(0.啟用 1.停用)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneContractDetailSub>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_ContractDetail_SUB");

                entity.HasIndex(e => new { e.CContractId, e.CSubContractId }, "NonClusteredIndex-20230517-113553");

                entity.Property(e => e.CId)
                    .HasColumnName("cID")
                    .HasComment("系統ID");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID")
                    .HasComment("文件編號");

                entity.Property(e => e.CSubContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cSubContractID")
                    .HasComment("下包文件編號");

                entity.Property(e => e.CSubNotes)
                    .HasMaxLength(255)
                    .HasColumnName("cSubNotes")
                    .HasComment("下包備註");

                entity.Property(e => e.CSubSupplierId)
                    .HasMaxLength(15)
                    .HasColumnName("cSubSupplierID")
                    .HasComment("下包商代號");

                entity.Property(e => e.CSubSupplierName)
                    .HasMaxLength(40)
                    .HasColumnName("cSubSupplierName")
                    .HasComment("下包商名稱");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.Disabled).HasComment("是否停用(0.啟用 1.停用)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneContractMain>(entity =>
            {
                entity.HasKey(e => e.CContractId);

                entity.ToTable("TB_ONE_ContractMain");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID")
                    .HasComment("文件編號");

                entity.Property(e => e.CBillCycle)
                    .HasMaxLength(512)
                    .HasColumnName("cBillCycle")
                    .HasComment("請款週期");

                entity.Property(e => e.CBillNotes)
                    .HasMaxLength(512)
                    .HasColumnName("cBillNotes")
                    .HasComment("請款備註");

                entity.Property(e => e.CContractNotes)
                    .HasMaxLength(512)
                    .HasColumnName("cContractNotes")
                    .HasComment("合約備註");

                entity.Property(e => e.CContractReport)
                    .HasColumnName("cContractReport")
                    .HasComment("合約書URL");

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID")
                    .HasComment("客戶代號");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(35)
                    .HasColumnName("cCustomerName")
                    .HasComment("客戶名稱");

                entity.Property(e => e.CDesc)
                    .HasMaxLength(255)
                    .HasColumnName("cDesc")
                    .HasComment("訂單說明");

                entity.Property(e => e.CEndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEndDate")
                    .HasComment("維護結束日期");

                entity.Property(e => e.CInvalidReason)
                    .HasMaxLength(255)
                    .HasColumnName("cInvalidReason");

                entity.Property(e => e.CIsSubContract)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsSubContract")
                    .HasComment("是否為下包約(Y、空白)");

                entity.Property(e => e.CMaaddress)
                    .HasMaxLength(255)
                    .HasColumnName("cMAAddress")
                    .HasComment("維護地址");

                entity.Property(e => e.CMacycle)
                    .HasMaxLength(512)
                    .HasColumnName("cMACycle")
                    .HasComment("維護週期");

                entity.Property(e => e.CManotes)
                    .HasMaxLength(512)
                    .HasColumnName("cMANotes")
                    .HasComment("維護備註");

                entity.Property(e => e.CMasales)
                    .HasMaxLength(20)
                    .HasColumnName("cMASales")
                    .HasComment("維護業務員ERPID");

                entity.Property(e => e.CMasalesName)
                    .HasMaxLength(40)
                    .HasColumnName("cMASalesName")
                    .HasComment("維護業務員姓名(中文+ 英文)");

                entity.Property(e => e.CSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("cSLARESP")
                    .HasComment("SLA回應條件");

                entity.Property(e => e.CSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("cSLASRV")
                    .HasComment("SLA服務條件");

                entity.Property(e => e.CSoNo)
                    .HasMaxLength(10)
                    .HasColumnName("cSoNo")
                    .HasComment("銷售訂單號");

                entity.Property(e => e.CSoSales)
                    .HasMaxLength(20)
                    .HasColumnName("cSoSales")
                    .HasComment("業務員ERPID");

                entity.Property(e => e.CSoSalesAss)
                    .HasMaxLength(20)
                    .HasColumnName("cSoSalesASS")
                    .HasComment("業務祕書ERPID");

                entity.Property(e => e.CSoSalesAssname)
                    .HasMaxLength(40)
                    .HasColumnName("cSoSalesASSName")
                    .HasComment("業務祕書姓名(中文+ 英文)");

                entity.Property(e => e.CSoSalesName)
                    .HasMaxLength(40)
                    .HasColumnName("cSoSalesName")
                    .HasComment("業務員姓名(中文+ 英文)");

                entity.Property(e => e.CStartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cStartDate")
                    .HasComment("維護開始日期");

                entity.Property(e => e.CTeamId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("cTeamID")
                    .HasComment("服務團隊ID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneDocument>(entity =>
            {
                entity.ToTable("TB_ONE_DOCUMENT");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.FileExt)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("FILE_EXT");

                entity.Property(e => e.FileName)
                    .HasMaxLength(50)
                    .HasColumnName("FILE_NAME");

                entity.Property(e => e.FileOrgName)
                    .HasMaxLength(100)
                    .HasColumnName("FILE_ORG_NAME");

                entity.Property(e => e.InsertTime)
                    .HasMaxLength(22)
                    .IsUnicode(false)
                    .HasColumnName("INSERT_TIME");

                entity.Property(e => e.RefObjId)
                    .HasMaxLength(50)
                    .HasColumnName("REF_OBJ_ID");
            });

            modelBuilder.Entity<TbOneLog>(entity =>
            {
                entity.HasKey(e => e.CId)
                    .HasName("PK_TB_PRO_LOG");

                entity.ToTable("TB_ONE_LOG");

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20221125-140220");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.EventName).HasMaxLength(200);
            });

            modelBuilder.Entity<TbOneSrcustomerEmailMapping>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRCustomerEmailMapping");

                entity.HasIndex(e => new { e.CCustomerId, e.CTeamId, e.CEmailId }, "NonClusteredIndex-20230206-182920");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CContactEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cContactEmail");

                entity.Property(e => e.CContactMobile)
                    .HasMaxLength(50)
                    .HasColumnName("cContactMobile");

                entity.Property(e => e.CContactName)
                    .HasMaxLength(40)
                    .HasColumnName("cContactName");

                entity.Property(e => e.CContactPhone)
                    .HasMaxLength(50)
                    .HasColumnName("cContactPhone");

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(35)
                    .HasColumnName("cCustomerName");

                entity.Property(e => e.CEmailId)
                    .HasMaxLength(50)
                    .HasColumnName("cEmailID");

                entity.Property(e => e.CTeamId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("cTeamID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrdetailContact>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRDetail_Contact");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CContactAddress)
                    .HasMaxLength(110)
                    .HasColumnName("cContactAddress");

                entity.Property(e => e.CContactEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cContactEmail");

                entity.Property(e => e.CContactMobile)
                    .HasMaxLength(50)
                    .HasColumnName("cContactMobile");

                entity.Property(e => e.CContactName)
                    .HasMaxLength(40)
                    .HasColumnName("cContactName");

                entity.Property(e => e.CContactPhone)
                    .HasMaxLength(50)
                    .HasColumnName("cContactPhone");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrdetailMaterialInfo>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRDetail_MaterialInfo");

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230418-143506");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CBasicContent).HasColumnName("cBasicContent");

                entity.Property(e => e.CBrand)
                    .HasMaxLength(80)
                    .HasColumnName("cBrand");

                entity.Property(e => e.CMaterialId)
                    .HasMaxLength(40)
                    .HasColumnName("cMaterialID");

                entity.Property(e => e.CMaterialName)
                    .HasMaxLength(255)
                    .HasColumnName("cMaterialName");

                entity.Property(e => e.CMfpnumber)
                    .HasMaxLength(50)
                    .HasColumnName("cMFPNumber");

                entity.Property(e => e.CProductHierarchy)
                    .HasMaxLength(18)
                    .HasColumnName("cProductHierarchy");

                entity.Property(e => e.CQuantity).HasColumnName("cQuantity");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrdetailPartsReplace>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRDetail_PartsReplace");

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20221115-165414");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CArriveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cArriveDate");

                entity.Property(e => e.CHpcaseId)
                    .HasMaxLength(30)
                    .HasColumnName("cHPCaseID");

                entity.Property(e => e.CHpct)
                    .HasMaxLength(20)
                    .HasColumnName("cHPCT");

                entity.Property(e => e.CMaterialId)
                    .HasMaxLength(40)
                    .HasColumnName("cMaterialID");

                entity.Property(e => e.CMaterialName)
                    .HasMaxLength(255)
                    .HasColumnName("cMaterialName");

                entity.Property(e => e.CNewCt)
                    .HasMaxLength(30)
                    .HasColumnName("cNewCT");

                entity.Property(e => e.CNewUefi)
                    .HasMaxLength(60)
                    .HasColumnName("cNewUEFI");

                entity.Property(e => e.CNote)
                    .HasMaxLength(255)
                    .HasColumnName("cNote");

                entity.Property(e => e.COldCt)
                    .HasMaxLength(30)
                    .HasColumnName("cOldCT");

                entity.Property(e => e.CPersonalDamage)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cPersonalDamage");

                entity.Property(e => e.CReturnDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cReturnDate");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CStandbySerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cStandbySerialID");

                entity.Property(e => e.CXchp)
                    .HasMaxLength(20)
                    .HasColumnName("cXCHP");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrdetailProduct>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRDetail_Product");

                entity.HasIndex(e => new { e.CSrid, e.CSerialId, e.CNewSerialId }, "NonClusteredIndex-20221108-160519");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CInstallId)
                    .HasMaxLength(20)
                    .HasColumnName("cInstallID");

                entity.Property(e => e.CMaterialId)
                    .HasMaxLength(20)
                    .HasColumnName("cMaterialID");

                entity.Property(e => e.CMaterialName)
                    .HasMaxLength(255)
                    .HasColumnName("cMaterialName");

                entity.Property(e => e.CNewSerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cNewSerialID");

                entity.Property(e => e.CProductNumber)
                    .HasMaxLength(50)
                    .HasColumnName("cProductNumber");

                entity.Property(e => e.CSerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cSerialID");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrdetailRecord>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRDetail_Record");

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20221108-160558");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CArriveTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cArriveTime");

                entity.Property(e => e.CDesc)
                    .HasMaxLength(255)
                    .HasColumnName("cDesc");

                entity.Property(e => e.CEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cEngineerID");

                entity.Property(e => e.CEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cEngineerName");

                entity.Property(e => e.CFinishTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cFinishTime");

                entity.Property(e => e.CReceiveTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cReceiveTime");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CSrreport).HasColumnName("cSRReport");

                entity.Property(e => e.CStartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cStartTime");

                entity.Property(e => e.CWorkHours)
                    .HasColumnType("numeric(6, 0)")
                    .HasColumnName("cWorkHours");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrdetailSerialFeedback>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRDetail_SerialFeedback");

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230418-143520");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CConfigReport).HasColumnName("cConfigReport");

                entity.Property(e => e.CMaterialId)
                    .HasMaxLength(20)
                    .HasColumnName("cMaterialID");

                entity.Property(e => e.CMaterialName)
                    .HasMaxLength(255)
                    .HasColumnName("cMaterialName");

                entity.Property(e => e.CSerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cSerialID");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrdetailWarranty>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRDetail_Warranty");

                entity.HasIndex(e => new { e.CSrid, e.CSerialId }, "NonClusteredIndex-20221108-160541");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CAdvice)
                    .HasMaxLength(2000)
                    .HasColumnName("cAdvice");

                entity.Property(e => e.CBpmformNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cBPMFormNo");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID");

                entity.Property(e => e.CSerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cSerialID");

                entity.Property(e => e.CSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("cSLARESP");

                entity.Property(e => e.CSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("cSLASRV");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CSubContractId)
                    .HasMaxLength(50)
                    .HasColumnName("cSubContractID");

                entity.Property(e => e.CUsed)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cUsed");

                entity.Property(e => e.CWtyedate)
                    .HasColumnType("datetime")
                    .HasColumnName("cWTYEDATE");

                entity.Property(e => e.CWtyid)
                    .HasMaxLength(40)
                    .HasColumnName("cWTYID");

                entity.Property(e => e.CWtyname)
                    .HasMaxLength(50)
                    .HasColumnName("cWTYName");

                entity.Property(e => e.CWtysdate)
                    .HasColumnType("datetime")
                    .HasColumnName("cWTYSDATE");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

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

                entity.Property(e => e.CAttachement).HasColumnName("cAttachement");

                entity.Property(e => e.CAttachementStockNo).HasColumnName("cAttachementStockNo");

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

                entity.Property(e => e.CFaultGroup)
                    .HasMaxLength(30)
                    .HasColumnName("cFaultGroup");

                entity.Property(e => e.CFaultGroupNote1)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNote1");

                entity.Property(e => e.CFaultGroupNote2)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNote2");

                entity.Property(e => e.CFaultGroupNote3)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNote3");

                entity.Property(e => e.CFaultGroupNote4)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNote4");

                entity.Property(e => e.CFaultGroupNoteOther)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNoteOther");

                entity.Property(e => e.CFaultSpec)
                    .HasMaxLength(30)
                    .HasColumnName("cFaultSpec");

                entity.Property(e => e.CFaultSpecNote1)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultSpecNote1");

                entity.Property(e => e.CFaultSpecNote2)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultSpecNote2");

                entity.Property(e => e.CFaultSpecNoteOther)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultSpecNoteOther");

                entity.Property(e => e.CFaultState)
                    .HasMaxLength(30)
                    .HasColumnName("cFaultState");

                entity.Property(e => e.CFaultStateNote1)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultStateNote1");

                entity.Property(e => e.CFaultStateNote2)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultStateNote2");

                entity.Property(e => e.CFaultStateNoteOther)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultStateNoteOther");

                entity.Property(e => e.CIsAppclose)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsAPPClose");

                entity.Property(e => e.CIsSecondFix)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsSecondFix");

                entity.Property(e => e.CMainEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cMainEngineerID");

                entity.Property(e => e.CMainEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cMainEngineerName");

                entity.Property(e => e.CMaserviceType)
                    .HasMaxLength(3)
                    .HasColumnName("cMAServiceType");

                entity.Property(e => e.CNotes)
                    .HasMaxLength(2000)
                    .HasColumnName("cNotes");

                entity.Property(e => e.CRepairAddress)
                    .HasMaxLength(110)
                    .HasColumnName("cRepairAddress");

                entity.Property(e => e.CRepairEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cRepairEmail");

                entity.Property(e => e.CRepairMobile)
                    .HasMaxLength(50)
                    .HasColumnName("cRepairMobile");

                entity.Property(e => e.CRepairName)
                    .HasMaxLength(40)
                    .HasColumnName("cRepairName");

                entity.Property(e => e.CRepairPhone)
                    .HasMaxLength(50)
                    .HasColumnName("cRepairPhone");

                entity.Property(e => e.CSalesId)
                    .HasMaxLength(20)
                    .HasColumnName("cSalesID");

                entity.Property(e => e.CSalesName)
                    .HasMaxLength(40)
                    .HasColumnName("cSalesName");

                entity.Property(e => e.CSalesNo)
                    .HasMaxLength(30)
                    .HasColumnName("cSalesNo");

                entity.Property(e => e.CSecretaryId)
                    .HasMaxLength(20)
                    .HasColumnName("cSecretaryID");

                entity.Property(e => e.CSecretaryName)
                    .HasMaxLength(40)
                    .HasColumnName("cSecretaryName");

                entity.Property(e => e.CShipmentNo)
                    .HasMaxLength(30)
                    .HasColumnName("cShipmentNo");

                entity.Property(e => e.CSqpersonId)
                    .HasMaxLength(6)
                    .HasColumnName("cSQPersonID");

                entity.Property(e => e.CSqpersonName)
                    .HasMaxLength(100)
                    .HasColumnName("cSQPersonName");

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
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("cTeamID");

                entity.Property(e => e.CTechManagerId)
                    .HasMaxLength(255)
                    .HasColumnName("cTechManagerID");

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

            modelBuilder.Entity<TbOneSrsqperson>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRSQPerson");

                entity.HasIndex(e => e.CFullKey, "NonClusteredIndex-20221129-100721");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CContent)
                    .HasMaxLength(30)
                    .HasColumnName("cContent");

                entity.Property(e => e.CEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cEngineerID");

                entity.Property(e => e.CEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cEngineerName");

                entity.Property(e => e.CFirstKey)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cFirstKEY");

                entity.Property(e => e.CFullKey)
                    .HasMaxLength(6)
                    .HasColumnName("cFullKEY");

                entity.Property(e => e.CFullName)
                    .HasMaxLength(100)
                    .HasColumnName("cFullNAME");

                entity.Property(e => e.CNo)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("cNO");

                entity.Property(e => e.CSecondKey)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cSecondKEY");

                entity.Property(e => e.CThirdKey)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cThirdKEY");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrteamMapping>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRTeamMapping");

                entity.HasIndex(e => new { e.CTeamNewId, e.CTeamOldId }, "NonClusteredIndex-20221108-160629");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CTeamNewId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("cTeamNewID");

                entity.Property(e => e.CTeamNewName)
                    .HasMaxLength(50)
                    .HasColumnName("cTeamNewName");

                entity.Property(e => e.CTeamOldId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .HasColumnName("cTeamOldID");

                entity.Property(e => e.CTeamOldName)
                    .HasMaxLength(50)
                    .HasColumnName("cTeamOldName");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<ViewOneSrreport>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("VIEW_ONE_SRREPORT");

                entity.Property(e => e.CArriveTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cArriveTime");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID");

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

                entity.Property(e => e.CDescR)
                    .HasMaxLength(255)
                    .HasColumnName("cDesc_R");

                entity.Property(e => e.CEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cEngineerID");

                entity.Property(e => e.CEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cEngineerName");

                entity.Property(e => e.CFinishTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cFinishTime");

                entity.Property(e => e.CHpct).HasColumnName("cHPCT");

                entity.Property(e => e.CIsAppclose)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsAPPClose");

                entity.Property(e => e.CIsSecondFix)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsSecondFix");

                entity.Property(e => e.CMaserviceType)
                    .HasMaxLength(9)
                    .HasColumnName("cMAServiceType");

                entity.Property(e => e.CMaterialId).HasColumnName("cMaterialID");

                entity.Property(e => e.CMaterialName).HasColumnName("cMaterialName");

                entity.Property(e => e.CNewCt).HasColumnName("cNewCT");

                entity.Property(e => e.CNotePr).HasColumnName("cNote_PR");

                entity.Property(e => e.CNotes)
                    .HasMaxLength(2000)
                    .HasColumnName("cNotes");

                entity.Property(e => e.COldCt).HasColumnName("cOldCT");

                entity.Property(e => e.CPersonalDamage).HasColumnName("cPersonalDamage");

                entity.Property(e => e.CReceiveTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cReceiveTime");

                entity.Property(e => e.CRepairAddress)
                    .HasMaxLength(110)
                    .HasColumnName("cRepairAddress");

                entity.Property(e => e.CRepairEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cRepairEmail");

                entity.Property(e => e.CRepairMobile)
                    .HasMaxLength(50)
                    .HasColumnName("cRepairMobile");

                entity.Property(e => e.CRepairName)
                    .HasMaxLength(40)
                    .HasColumnName("cRepairName");

                entity.Property(e => e.CRepairPhone)
                    .HasMaxLength(50)
                    .HasColumnName("cRepairPhone");

                entity.Property(e => e.CSerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cSerialID");

                entity.Property(e => e.CSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("cSLARESP");

                entity.Property(e => e.CSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("cSLASRV");

                entity.Property(e => e.CSqpersonName)
                    .HasMaxLength(100)
                    .HasColumnName("cSQPersonName");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CSrprocessWay)
                    .HasMaxLength(4)
                    .HasColumnName("cSRProcessWay");

                entity.Property(e => e.CSrreport).HasColumnName("cSRReport");

                entity.Property(e => e.CSrtype)
                    .HasMaxLength(4)
                    .HasColumnName("cSRType");

                entity.Property(e => e.CSrtypeNote)
                    .HasMaxLength(9)
                    .HasColumnName("cSRTypeNote");

                entity.Property(e => e.CSrtypeOne)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeOne");

                entity.Property(e => e.CSrtypeOneNote)
                    .HasMaxLength(100)
                    .HasColumnName("cSRTypeOneNote");

                entity.Property(e => e.CSrtypeSec)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeSec");

                entity.Property(e => e.CSrtypeSecNote)
                    .HasMaxLength(100)
                    .HasColumnName("cSRTypeSecNote");

                entity.Property(e => e.CSrtypeThr)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeThr");

                entity.Property(e => e.CSrtypeThrNote)
                    .HasMaxLength(100)
                    .HasColumnName("cSRTypeThrNote");

                entity.Property(e => e.CStartTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cStartTime");

                entity.Property(e => e.CStatus)
                    .HasMaxLength(5)
                    .HasColumnName("cStatus");

                entity.Property(e => e.CStatusNote)
                    .HasMaxLength(12)
                    .HasColumnName("cStatusNote");

                entity.Property(e => e.CTeamId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("cTeamID");

                entity.Property(e => e.CWorkHours)
                    .HasColumnType("numeric(6, 0)")
                    .HasColumnName("cWorkHours");

                entity.Property(e => e.CWtyedate)
                    .HasColumnType("datetime")
                    .HasColumnName("cWTYEDATE");

                entity.Property(e => e.CWtyid)
                    .HasMaxLength(40)
                    .HasColumnName("cWTYID");

                entity.Property(e => e.CWtyname)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("cWTYName");

                entity.Property(e => e.CWtysdate)
                    .HasColumnType("datetime")
                    .HasColumnName("cWTYSDATE");

                entity.Property(e => e.CXchp).HasColumnName("cXCHP");

                entity.Property(e => e.CountIn)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CountIN");

                entity.Property(e => e.CountOut)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CountOUT");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Dn)
                    .HasMaxLength(30)
                    .HasColumnName("DN");

                entity.Property(e => e.Pid)
                    .HasMaxLength(255)
                    .HasColumnName("PID");

                entity.Property(e => e.Pn)
                    .HasMaxLength(50)
                    .HasColumnName("PN");

                entity.Property(e => e.So)
                    .HasMaxLength(30)
                    .HasColumnName("SO");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
