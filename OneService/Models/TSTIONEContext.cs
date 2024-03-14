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
        public virtual DbSet<TbOneSrbatchInstallRecord> TbOneSrbatchInstallRecords { get; set; } = null!;
        public virtual DbSet<TbOneSrbatchInstallRecordDetail> TbOneSrbatchInstallRecordDetails { get; set; } = null!;
        public virtual DbSet<TbOneSrbatchMaintainRecord> TbOneSrbatchMaintainRecords { get; set; } = null!;
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
        public virtual DbSet<TbOneSroftenUsedDatum> TbOneSroftenUsedData { get; set; } = null!;
        public virtual DbSet<TbOneSrrepairType> TbOneSrrepairTypes { get; set; } = null!;
        public virtual DbSet<TbOneSrsatisfactionSurveyRemove> TbOneSrsatisfactionSurveyRemoves { get; set; } = null!;
        public virtual DbSet<TbOneSrsqperson> TbOneSrsqpeople { get; set; } = null!;
        public virtual DbSet<TbOneSrteamMapping> TbOneSrteamMappings { get; set; } = null!;
        public virtual DbSet<ViewOneSrreport> ViewOneSrreports { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.31.7.54;Database=TSTI-ONE;User=TSTI-ONE;Password=!QAZ5tgb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbOneContractDetailEng>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_ContractDetail_ENG");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CContactStoreId).HasColumnName("cContactStoreID");

                entity.Property(e => e.CContactStoreName)
                    .HasMaxLength(40)
                    .HasColumnName("cContactStoreName");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID");

                entity.Property(e => e.CEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cEngineerID");

                entity.Property(e => e.CEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cEngineerName");

                entity.Property(e => e.CIsMainEngineer)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsMainEngineer");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneContractDetailObj>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_ContractDetail_OBJ");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CAddress)
                    .HasMaxLength(100)
                    .HasColumnName("cAddress");

                entity.Property(e => e.CArea)
                    .HasMaxLength(30)
                    .HasColumnName("cArea");

                entity.Property(e => e.CBrands)
                    .HasMaxLength(30)
                    .HasColumnName("cBrands");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID");

                entity.Property(e => e.CHostName)
                    .HasMaxLength(120)
                    .HasColumnName("cHostName");

                entity.Property(e => e.CLocation)
                    .HasMaxLength(100)
                    .HasColumnName("cLocation");

                entity.Property(e => e.CModel)
                    .HasMaxLength(100)
                    .HasColumnName("cModel");

                entity.Property(e => e.CNotes)
                    .HasMaxLength(255)
                    .HasColumnName("cNotes");

                entity.Property(e => e.CPid)
                    .HasMaxLength(40)
                    .HasColumnName("cPID");

                entity.Property(e => e.CSerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cSerialID");

                entity.Property(e => e.CSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("cSLARESP");

                entity.Property(e => e.CSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("cSLASRV");

                entity.Property(e => e.CSubContractId)
                    .HasMaxLength(50)
                    .HasColumnName("cSubContractID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneContractDetailSub>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_ContractDetail_SUB");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID");

                entity.Property(e => e.CSubContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cSubContractID");

                entity.Property(e => e.CSubNotes)
                    .HasMaxLength(1000)
                    .HasColumnName("cSubNotes");

                entity.Property(e => e.CSubSupplierId)
                    .HasMaxLength(15)
                    .HasColumnName("cSubSupplierID");

                entity.Property(e => e.CSubSupplierName)
                    .HasMaxLength(40)
                    .HasColumnName("cSubSupplierName");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneContractMain>(entity =>
            {
                entity.HasKey(e => e.CContractId);

                entity.ToTable("TB_ONE_ContractMain");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID");

                entity.Property(e => e.CBillCycle)
                    .HasMaxLength(1000)
                    .HasColumnName("cBillCycle");

                entity.Property(e => e.CBillNotes)
                    .HasMaxLength(1000)
                    .HasColumnName("cBillNotes");

                entity.Property(e => e.CContactEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cContactEmail");

                entity.Property(e => e.CContactName)
                    .HasMaxLength(40)
                    .HasColumnName("cContactName");

                entity.Property(e => e.CContactPhone)
                    .HasMaxLength(50)
                    .HasColumnName("cContactPhone");

                entity.Property(e => e.CContractNotes).HasColumnName("cContractNotes");

                entity.Property(e => e.CContractReport).HasColumnName("cContractReport");

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(255)
                    .HasColumnName("cCustomerName");

                entity.Property(e => e.CDesc)
                    .HasMaxLength(512)
                    .HasColumnName("cDesc");

                entity.Property(e => e.CEndDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cEndDate");

                entity.Property(e => e.CInvalidReason)
                    .HasMaxLength(255)
                    .HasColumnName("cInvalidReason");

                entity.Property(e => e.CIsSubContract)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsSubContract");

                entity.Property(e => e.CMaaddress)
                    .HasMaxLength(255)
                    .HasColumnName("cMAAddress");

                entity.Property(e => e.CMacycle)
                    .HasMaxLength(1000)
                    .HasColumnName("cMACycle");

                entity.Property(e => e.CManotes)
                    .HasMaxLength(1000)
                    .HasColumnName("cMANotes");

                entity.Property(e => e.CMasales)
                    .HasMaxLength(20)
                    .HasColumnName("cMASales");

                entity.Property(e => e.CMasalesName)
                    .HasMaxLength(40)
                    .HasColumnName("cMASalesName");

                entity.Property(e => e.CSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("cSLARESP");

                entity.Property(e => e.CSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("cSLASRV");

                entity.Property(e => e.CSoNo)
                    .HasMaxLength(10)
                    .HasColumnName("cSoNo");

                entity.Property(e => e.CSoSales)
                    .HasMaxLength(20)
                    .HasColumnName("cSoSales");

                entity.Property(e => e.CSoSalesAss)
                    .HasMaxLength(20)
                    .HasColumnName("cSoSalesASS");

                entity.Property(e => e.CSoSalesAssname)
                    .HasMaxLength(40)
                    .HasColumnName("cSoSalesASSName");

                entity.Property(e => e.CSoSalesName)
                    .HasMaxLength(40)
                    .HasColumnName("cSoSalesName");

                entity.Property(e => e.CStartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cStartDate");

                entity.Property(e => e.CTeamId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("cTeamID");

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
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_LOG");

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230322-165229");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.EventName).HasMaxLength(200);
            });

            modelBuilder.Entity<TbOneSrbatchInstallRecord>(entity =>
            {
                entity.HasKey(e => new { e.CGuid, e.CSrid });

                entity.ToTable("TB_ONE_SRBatchInstallRecord");

                entity.Property(e => e.CGuid).HasColumnName("cGUID");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

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

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(35)
                    .HasColumnName("cCustomerName");

                entity.Property(e => e.CMainEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cMainEngineerID");

                entity.Property(e => e.CMainEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cMainEngineerName");

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

                entity.Property(e => e.CSerialId)
                    .HasMaxLength(40)
                    .HasColumnName("cSerialID");

                entity.Property(e => e.CShipmentNo)
                    .HasMaxLength(30)
                    .HasColumnName("cShipmentNo");

                entity.Property(e => e.CTeamId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("cTeamID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrbatchInstallRecordDetail>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRBatchInstallRecord_Detail");

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230711-132408");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CMaterialId)
                    .HasMaxLength(40)
                    .HasColumnName("cMaterialID");

                entity.Property(e => e.CMaterialName)
                    .HasMaxLength(255)
                    .HasColumnName("cMaterialName");

                entity.Property(e => e.CQuantity).HasColumnName("cQuantity");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrbatchMaintainRecord>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRBatchMaintainRecord");

                entity.HasIndex(e => e.CContractId, "NonClusteredIndex-20230713-173349");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CBukrs)
                    .HasMaxLength(4)
                    .HasColumnName("cBUKRS");

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

                entity.Property(e => e.CContactStoreName)
                    .HasMaxLength(40)
                    .HasColumnName("cContactStoreName");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID");

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(35)
                    .HasColumnName("cCustomerName");

                entity.Property(e => e.CDisabled).HasColumnName("cDisabled");

                entity.Property(e => e.CMacycle)
                    .HasMaxLength(512)
                    .HasColumnName("cMACycle");

                entity.Property(e => e.CMainEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cMainEngineerID");

                entity.Property(e => e.CMainEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cMainEngineerName");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrcustomerEmailMapping>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRCustomerEmailMapping");

                entity.HasIndex(e => new { e.CCustomerId, e.CTeamId, e.CEmailId }, "NonClusteredIndex-20230322-165135");

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

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230322-164917");

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

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230418-143646");

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

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230322-165049");

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

                entity.HasIndex(e => new { e.CSrid, e.CSerialId, e.CNewSerialId }, "NonClusteredIndex-20230322-164937");

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

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230322-165024");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CArriveTime)
                    .HasColumnType("datetime")
                    .HasColumnName("cArriveTime");

                entity.Property(e => e.CDesc)
                    .HasMaxLength(4000)
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

                entity.HasIndex(e => e.CSrid, "NonClusteredIndex-20230418-143632");

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

                entity.HasIndex(e => new { e.CSrid, e.CSerialId }, "NonClusteredIndex-20230322-164957");

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
                    .HasMaxLength(255)
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
                    .HasColumnName("cSRID")
                    .HasComment("服務ID");

                entity.Property(e => e.CAssEngineerId)
                    .HasMaxLength(255)
                    .HasColumnName("cAssEngineerID")
                    .HasComment("協助工程師ERPID");

                entity.Property(e => e.CAttachement)
                    .HasColumnName("cAttachement")
                    .HasComment("檢附文件");

                entity.Property(e => e.CAttachementStockNo)
                    .HasColumnName("cAttachementStockNo")
                    .HasComment("備料服務通知單文件");

                entity.Property(e => e.CContractId)
                    .HasMaxLength(10)
                    .HasColumnName("cContractID");

                entity.Property(e => e.CCountIn).HasColumnName("cCountIN");

                entity.Property(e => e.CCountOut).HasColumnName("cCountOUT");

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID")
                    .HasComment("客戶代號");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(35)
                    .HasColumnName("cCustomerName")
                    .HasComment("客戶名稱");

                entity.Property(e => e.CCustomerUnitType)
                    .HasMaxLength(3)
                    .HasColumnName("cCustomerUnitType");

                entity.Property(e => e.CDelayReason)
                    .HasMaxLength(1000)
                    .HasColumnName("cDelayReason")
                    .HasComment("延遲結案原因");

                entity.Property(e => e.CDesc)
                    .HasMaxLength(255)
                    .HasColumnName("cDesc")
                    .HasComment("說明");

                entity.Property(e => e.CFaultGroup)
                    .HasMaxLength(30)
                    .HasColumnName("cFaultGroup")
                    .HasComment("客戶故障狀況分類");

                entity.Property(e => e.CFaultGroupNote1)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNote1")
                    .HasComment("客戶故障狀況分類說明-硬體");

                entity.Property(e => e.CFaultGroupNote2)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNote2")
                    .HasComment("客戶故障狀況分類說明-系統");

                entity.Property(e => e.CFaultGroupNote3)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNote3")
                    .HasComment("客戶故障狀況分類說明-服務");

                entity.Property(e => e.CFaultGroupNote4)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNote4")
                    .HasComment("客戶故障狀況分類說明-網路");

                entity.Property(e => e.CFaultGroupNoteOther)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultGroupNoteOther")
                    .HasComment("客戶故障狀況分類說明-其他");

                entity.Property(e => e.CFaultSpec)
                    .HasMaxLength(30)
                    .HasColumnName("cFaultSpec")
                    .HasComment("故障零件規格料號");

                entity.Property(e => e.CFaultSpecNote1)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultSpecNote1")
                    .HasComment("故障零件規格料號-零件規格");

                entity.Property(e => e.CFaultSpecNote2)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultSpecNote2")
                    .HasComment("故障零件規格料號-零件料號");

                entity.Property(e => e.CFaultSpecNoteOther)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultSpecNoteOther")
                    .HasComment("故障零件規格料號-其他");

                entity.Property(e => e.CFaultState)
                    .HasMaxLength(30)
                    .HasColumnName("cFaultState")
                    .HasComment("客戶故障狀況");

                entity.Property(e => e.CFaultStateNote1)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultStateNote1")
                    .HasComment("客戶故障狀況說明-面板燈號");

                entity.Property(e => e.CFaultStateNote2)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultStateNote2")
                    .HasComment("客戶故障狀況說明-管理介面(iLO、IMM、iDRAC)");

                entity.Property(e => e.CFaultStateNoteOther)
                    .HasMaxLength(100)
                    .HasColumnName("cFaultStateNoteOther")
                    .HasComment("客戶故障狀況說明-其他");

                entity.Property(e => e.CIsAppclose)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsAPPClose")
                    .HasComment("是否為APP結案");

                entity.Property(e => e.CIsInternalWork)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsInternalWork");

                entity.Property(e => e.CIsSecondFix)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cIsSecondFix")
                    .HasComment("是否為二修");

                entity.Property(e => e.CMainEngineerId)
                    .HasMaxLength(20)
                    .HasColumnName("cMainEngineerID")
                    .HasComment("主要工程師ERPID");

                entity.Property(e => e.CMainEngineerName)
                    .HasMaxLength(40)
                    .HasColumnName("cMainEngineerName")
                    .HasComment("主要工程師姓名");

                entity.Property(e => e.CMaserviceType)
                    .HasMaxLength(3)
                    .HasColumnName("cMAServiceType")
                    .HasComment("維護服務種類");

                entity.Property(e => e.CNotes)
                    .HasMaxLength(2000)
                    .HasColumnName("cNotes")
                    .HasComment("詳細描述");

                entity.Property(e => e.CPerCallSlaresp)
                    .HasMaxLength(10)
                    .HasColumnName("cPerCallSLARESP");

                entity.Property(e => e.CPerCallSlasrv)
                    .HasMaxLength(10)
                    .HasColumnName("cPerCallSLASRV");

                entity.Property(e => e.CRemark).HasColumnName("cRemark");

                entity.Property(e => e.CRepairAddress)
                    .HasMaxLength(110)
                    .HasColumnName("cRepairAddress")
                    .HasComment("客戶報修人地址");

                entity.Property(e => e.CRepairEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cRepairEmail")
                    .HasComment("客戶報修人Email");

                entity.Property(e => e.CRepairMobile)
                    .HasMaxLength(50)
                    .HasColumnName("cRepairMobile")
                    .HasComment("客戶報修人手機");

                entity.Property(e => e.CRepairName)
                    .HasMaxLength(40)
                    .HasColumnName("cRepairName")
                    .HasComment("客戶報修人姓名");

                entity.Property(e => e.CRepairPhone)
                    .HasMaxLength(50)
                    .HasColumnName("cRepairPhone")
                    .HasComment("客戶報修人電話");

                entity.Property(e => e.CSalesId)
                    .HasMaxLength(20)
                    .HasColumnName("cSalesID")
                    .HasComment("計費業務ERPID");

                entity.Property(e => e.CSalesName)
                    .HasMaxLength(40)
                    .HasColumnName("cSalesName")
                    .HasComment("計費業務姓名");

                entity.Property(e => e.CSalesNo)
                    .HasMaxLength(30)
                    .HasColumnName("cSalesNo")
                    .HasComment("銷售訂單號");

                entity.Property(e => e.CScheduleDate)
                    .HasColumnType("datetime")
                    .HasColumnName("cScheduleDate");

                entity.Property(e => e.CSecretaryId)
                    .HasMaxLength(20)
                    .HasColumnName("cSecretaryID")
                    .HasComment("業務祕書ERPID");

                entity.Property(e => e.CSecretaryName)
                    .HasMaxLength(40)
                    .HasColumnName("cSecretaryName")
                    .HasComment("業務祕書姓名");

                entity.Property(e => e.CShipmentNo)
                    .HasMaxLength(30)
                    .HasColumnName("cShipmentNo")
                    .HasComment("出貨單號");

                entity.Property(e => e.CSqpersonId)
                    .HasMaxLength(6)
                    .HasColumnName("cSQPersonID")
                    .HasComment("SQ人員ID");

                entity.Property(e => e.CSqpersonName)
                    .HasMaxLength(100)
                    .HasColumnName("cSQPersonName")
                    .HasComment("SQ人員名稱");

                entity.Property(e => e.CSrpathWay)
                    .HasMaxLength(3)
                    .HasColumnName("cSRPathWay")
                    .HasComment("報修管道");

                entity.Property(e => e.CSrprocessWay)
                    .HasMaxLength(3)
                    .HasColumnName("cSRProcessWay")
                    .HasComment("處理方式");

                entity.Property(e => e.CSrrepairLevel)
                    .HasMaxLength(3)
                    .HasColumnName("cSRRepairLevel");

                entity.Property(e => e.CSrtypeOne)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeOne")
                    .HasComment("第一階(大類)");

                entity.Property(e => e.CSrtypeSec)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeSec")
                    .HasComment("第二階(中類)");

                entity.Property(e => e.CSrtypeThr)
                    .HasMaxLength(20)
                    .HasColumnName("cSRTypeThr")
                    .HasComment("第三階(小類)");

                entity.Property(e => e.CStatus)
                    .HasMaxLength(5)
                    .HasColumnName("cStatus")
                    .HasComment("狀態");

                entity.Property(e => e.CSystemGuid)
                    .HasColumnName("cSystemGUID")
                    .HasComment("系統目前GUID");

                entity.Property(e => e.CTeamId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("cTeamID")
                    .HasComment("服務團隊ID");

                entity.Property(e => e.CTechManagerId)
                    .HasMaxLength(255)
                    .HasColumnName("cTechManagerID")
                    .HasComment("技術主管ERPID");

                entity.Property(e => e.CTechTeamId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("cTechTeamID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSroftenUsedDatum>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SROftenUsedData");

                entity.HasIndex(e => new { e.CFunctionId, e.CCompanyId, e.CNo, e.CreatedErpid }, "NonClusteredIndex-20230809-172232");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CCompanyId)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("cCompanyID");

                entity.Property(e => e.CDescription)
                    .HasMaxLength(255)
                    .HasColumnName("cDescription");

                entity.Property(e => e.CFunctionId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("cFunctionID");

                entity.Property(e => e.CNo)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("cNo");

                entity.Property(e => e.CValue)
                    .HasMaxLength(2000)
                    .HasColumnName("cValue");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedErpid)
                    .HasMaxLength(20)
                    .HasColumnName("CreatedERPID");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrrepairType>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRRepairType");

                entity.HasIndex(e => e.CKindKey, "NonClusteredIndex-20230322-165202");

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

            modelBuilder.Entity<TbOneSrsatisfactionSurveyRemove>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRSatisfactionSurveyRemove");

                entity.HasIndex(e => new { e.CCustomerId, e.CContactEmail }, "NonClusteredIndex-20230825-093319");

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CContactEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cContactEmail");

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(35)
                    .HasColumnName("cCustomerName");

                entity.Property(e => e.CDimension)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("cDimension");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedUserName).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedUserName).HasMaxLength(50);
            });

            modelBuilder.Entity<TbOneSrsqperson>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("TB_ONE_SRSQPerson");

                entity.HasIndex(e => e.CFullKey, "NonClusteredIndex-20230322-165254");

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

                entity.HasIndex(e => new { e.CTeamNewId, e.CTeamOldId }, "NonClusteredIndex-20230322-165113");

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
                    .HasMaxLength(20)
                    .HasColumnName("cContractID");

                entity.Property(e => e.CCustomerId)
                    .HasMaxLength(10)
                    .HasColumnName("cCustomerID");

                entity.Property(e => e.CCustomerName)
                    .HasMaxLength(40)
                    .HasColumnName("cCustomerName");

                entity.Property(e => e.CCustomerUnitType)
                    .HasMaxLength(3)
                    .HasColumnName("cCustomerUnitType");

                entity.Property(e => e.CDelayReason)
                    .HasMaxLength(1000)
                    .HasColumnName("cDelayReason");

                entity.Property(e => e.CDesc)
                    .HasMaxLength(255)
                    .HasColumnName("cDesc");

                entity.Property(e => e.CDescR)
                    .HasMaxLength(4000)
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
                    .HasMaxLength(2)
                    .HasColumnName("cIsSecondFix");

                entity.Property(e => e.CMaserviceType)
                    .HasMaxLength(20)
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
                    .HasMaxLength(250)
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
                    .HasMaxLength(500)
                    .HasColumnName("cSerialID");

                entity.Property(e => e.CSlaresp)
                    .HasMaxLength(20)
                    .HasColumnName("cSLARESP");

                entity.Property(e => e.CSlasrv)
                    .HasMaxLength(20)
                    .HasColumnName("cSLASRV");

                entity.Property(e => e.CSqpersonName)
                    .HasMaxLength(100)
                    .HasColumnName("cSQPersonName");

                entity.Property(e => e.CSrid)
                    .HasMaxLength(20)
                    .HasColumnName("cSRID");

                entity.Property(e => e.CSrprocessWay)
                    .HasMaxLength(20)
                    .HasColumnName("cSRProcessWay");

                entity.Property(e => e.CSrreport).HasColumnName("cSRReport");

                entity.Property(e => e.CSrtype)
                    .HasMaxLength(4)
                    .HasColumnName("cSRType");

                entity.Property(e => e.CSrtypeNote)
                    .HasMaxLength(40)
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
                    .HasMaxLength(40)
                    .HasColumnName("cStatusNote");

                entity.Property(e => e.CTeamId)
                    .HasMaxLength(255)
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
                    .HasMaxLength(40)
                    .HasColumnName("cWTYName");

                entity.Property(e => e.CWtysdate)
                    .HasColumnType("datetime")
                    .HasColumnName("cWTYSDATE");

                entity.Property(e => e.CXchp).HasColumnName("cXCHP");

                entity.Property(e => e.CountIn)
                    .HasMaxLength(20)
                    .HasColumnName("CountIN");

                entity.Property(e => e.CountOut)
                    .HasMaxLength(20)
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
