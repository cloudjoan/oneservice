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

        public virtual DbSet<TbOneDocument> TbOneDocuments { get; set; } = null!;
        public virtual DbSet<TbOneLog> TbOneLogs { get; set; } = null!;
        public virtual DbSet<TbOneSrcustomerEmailMapping> TbOneSrcustomerEmailMappings { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailPartsReplace> TbOneSrdetailPartsReplaces { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailProduct> TbOneSrdetailProducts { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailRecord> TbOneSrdetailRecords { get; set; } = null!;
        public virtual DbSet<TbOneSrdetailWarranty> TbOneSrdetailWarranties { get; set; } = null!;
        public virtual DbSet<TbOneSridformat> TbOneSridformats { get; set; } = null!;
        public virtual DbSet<TbOneSrmain> TbOneSrmains { get; set; } = null!;
        public virtual DbSet<TbOneSrrepairType> TbOneSrrepairTypes { get; set; } = null!;
        public virtual DbSet<TbOneSrsqperson> TbOneSrsqpeople { get; set; } = null!;
        public virtual DbSet<TbOneSrteamMapping> TbOneSrteamMappings { get; set; } = null!;

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

                entity.Property(e => e.CId).HasColumnName("cID");

                entity.Property(e => e.CContactEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cContactEmail");

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
                    .HasMaxLength(40)
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

                entity.HasIndex(e => new { e.CSrid, e.CSerialId }, "NonClusteredIndex-20221108-160519");

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

                entity.Property(e => e.CAttatchement).HasColumnName("cAttatchement");

                entity.Property(e => e.CContactAddress)
                    .HasMaxLength(110)
                    .HasColumnName("cContactAddress");

                entity.Property(e => e.CContactEmail)
                    .HasMaxLength(200)
                    .HasColumnName("cContactEmail");

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

                entity.Property(e => e.CDelayReason)
                    .HasMaxLength(255)
                    .HasColumnName("cDelayReason");

                entity.Property(e => e.CDesc)
                    .HasMaxLength(255)
                    .HasColumnName("cDesc");

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
