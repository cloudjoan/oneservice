using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class F4501
    {
        /// <summary>
        /// 項次(系統自動產生)
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 公司別
        /// </summary>
        public string? CompCde { get; set; }
        /// <summary>
        /// 文件性質
        /// </summary>
        public string? Nature { get; set; }
        /// <summary>
        /// 文件編號
        /// </summary>
        public string No { get; set; } = null!;
        /// <summary>
        /// 年度
        /// </summary>
        public string? Year { get; set; }
        /// <summary>
        /// 文件類型
        /// </summary>
        public string? Ctype { get; set; }
        /// <summary>
        /// 文件對象
        /// </summary>
        public string? Target { get; set; }
        /// <summary>
        /// 文件期間
        /// </summary>
        public DateTime? Cdate { get; set; }
        /// <summary>
        /// 合約開始日期
        /// </summary>
        public DateTime? StrDate { get; set; }
        /// <summary>
        /// 合約終止日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 合約金額
        /// </summary>
        public int? Camt { get; set; }
        /// <summary>
        /// 收發文字號
        /// </summary>
        public string? Crno { get; set; }
        /// <summary>
        /// 文件名稱(案名)
        /// </summary>
        public string? Cname { get; set; }
        /// <summary>
        /// 文件內容簡述(目的)
        /// </summary>
        public string? Ccontent { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 文件業務代表
        /// </summary>
        public string? Sales { get; set; }
        /// <summary>
        /// 文件承辦人
        /// </summary>
        public string? Cpic { get; set; }
        public string? Ocpic { get; set; }
        /// <summary>
        /// 文件保管單位
        /// </summary>
        public string? Cdept { get; set; }
        /// <summary>
        /// 文件機密等級
        /// </summary>
        public string? Classified { get; set; }
        /// <summary>
        /// 文件速別
        /// </summary>
        public string? First { get; set; }
        /// <summary>
        /// BPM表單編號
        /// </summary>
        public string? Bpmno { get; set; }
        /// <summary>
        /// 文件類型(其他說明)
        /// </summary>
        public string? CtypeOther { get; set; }
        /// <summary>
        /// 流程狀態 - [空白]：正常，[S]：用印，[F]：結案，[I]：作廢
        /// </summary>
        public string? Flag { get; set; }
        /// <summary>
        /// 申請日期
        /// </summary>
        public DateTime? Adate { get; set; }
        /// <summary>
        /// 申請人員工編號
        /// </summary>
        public int? Aeid { get; set; }
        /// <summary>
        /// 申請人員工ID(SAP)
        /// </summary>
        public string? Asapid { get; set; }
        /// <summary>
        /// 申請人職稱
        /// </summary>
        public string? Atitle { get; set; }
        /// <summary>
        /// 申請人帳號
        /// </summary>
        public string? Aaccount { get; set; }
        /// <summary>
        /// 申請人單位
        /// </summary>
        public string? Adept { get; set; }
        /// <summary>
        /// 申請人單位ID
        /// </summary>
        public string? AdeptId { get; set; }
        /// <summary>
        /// 申請人利潤中心
        /// </summary>
        public string? Apc { get; set; }
        /// <summary>
        /// 申請人成本中心
        /// </summary>
        public string? Acc { get; set; }
        /// <summary>
        /// 申請人工作地點
        /// </summary>
        public string? Awp { get; set; }
        /// <summary>
        /// 填表人員工編號
        /// </summary>
        public int? Feid { get; set; }
        /// <summary>
        /// 填表人員工ID
        /// </summary>
        public string? Fsapid { get; set; }
        /// <summary>
        /// 填表人職稱
        /// </summary>
        public string? Ftitle { get; set; }
        /// <summary>
        /// 填表人帳號
        /// </summary>
        public string? Faccount { get; set; }
        /// <summary>
        /// 填表人單位
        /// </summary>
        public string? Fdept { get; set; }
        /// <summary>
        /// 填表人單位ID
        /// </summary>
        public string? FdeptId { get; set; }
        /// <summary>
        /// 填表人利潤中心
        /// </summary>
        public string? Fpc { get; set; }
        /// <summary>
        /// 填表人成本中心
        /// </summary>
        public string? Fcc { get; set; }
        /// <summary>
        /// 填表人工作地點
        /// </summary>
        public string? Fwp { get; set; }
        /// <summary>
        /// 審核方式(相關文件審核)
        /// </summary>
        public string? AppType { get; set; }
        /// <summary>
        /// 公司大章
        /// </summary>
        public string? Bs { get; set; }
        /// <summary>
        /// 小章
        /// </summary>
        public string? Ss { get; set; }
        /// <summary>
        /// 簽名
        /// </summary>
        public string? Sign { get; set; }
        /// <summary>
        /// 其他章
        /// </summary>
        public string? Os { get; set; }
        /// <summary>
        /// 騎縫章
        /// </summary>
        public string? Sp { get; set; }
        /// <summary>
        /// 投標專用章
        /// </summary>
        public string? Cs { get; set; }
        /// <summary>
        /// 負責人身份證影本
        /// </summary>
        public string? Idcard { get; set; }
        /// <summary>
        /// 份數
        /// </summary>
        public string? Pts { get; set; }
        /// <summary>
        /// 履約保證金
        /// </summary>
        public string? Csd { get; set; }
        /// <summary>
        /// 預期一天罰金為總金額之佔比
        /// </summary>
        public string? Bc { get; set; }
        /// <summary>
        /// 收付款條件
        /// </summary>
        public string? Cp { get; set; }
        /// <summary>
        /// 設備標的
        /// </summary>
        public string? Fo { get; set; }
        /// <summary>
        /// 糾紛處理
        /// </summary>
        public string? Dp { get; set; }
        /// <summary>
        /// 糾紛處理地點
        /// </summary>
        public string? Dplace { get; set; }
        /// <summary>
        /// 到期自動延長日期
        /// </summary>
        public DateTime? Extend { get; set; }
        /// <summary>
        /// 保固期間
        /// </summary>
        public string? Wp { get; set; }
        /// <summary>
        /// 會簽單位
        /// </summary>
        public string? CounterSign { get; set; }
        /// <summary>
        /// 印花稅總額
        /// </summary>
        public int? TaxAmt { get; set; }
        /// <summary>
        /// 結案日期
        /// </summary>
        public DateTime? CloseDate { get; set; }
        /// <summary>
        /// ECS申請號
        /// </summary>
        public string? EcsapplyNo { get; set; }
        /// <summary>
        /// 作廢原因
        /// </summary>
        public string? Cvr { get; set; }
        public string? UploadFiles { get; set; }
        /// <summary>
        /// 簽約對象身分
        /// </summary>
        public string? Vendor { get; set; }
        /// <summary>
        /// 建檔者帳號/姓名
        /// </summary>
        public string? CrUser { get; set; }
        /// <summary>
        /// 建檔日期/時間
        /// </summary>
        public DateTime? CrDate { get; set; }
        /// <summary>
        /// 最近異動者帳號/姓名
        /// </summary>
        public string? Userstamp { get; set; }
        /// <summary>
        /// 最近異動日期/時間
        /// </summary>
        public DateTime? Datestamp { get; set; }
        /// <summary>
        /// 商機號碼
        /// </summary>
        public string? CrmOppNo { get; set; }
        /// <summary>
        /// 請款周期
        /// </summary>
        public string? BillableTime { get; set; }
        /// <summary>
        /// 業務祕書ERPID
        /// </summary>
        public string? ErpAssid { get; set; }
    }
}
