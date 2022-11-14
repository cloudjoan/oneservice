using Microsoft.AspNetCore.Mvc;
using OneService.Models;
using System.Security.Principal;

namespace OneService.Controllers
{
    public class CommonFunction
    {
        PSIPContext psipDb = new PSIPContext();
        TAIFContext bpmDB = new TAIFContext();
        ERP_PROXY_DBContext dbProxy = new ERP_PROXY_DBContext();
        MCSWorkflowContext dbEIP = new MCSWorkflowContext();

        #region -----↓↓↓↓↓共用方法 ↓↓↓↓↓-----

        #region 取得登入者資訊
        public EmployeeBean findEmployeeInfo(string keyword)
        {
            EmployeeBean empBean = new EmployeeBean();

            var beanE = dbEIP.People.FirstOrDefault(x => x.Account.ToLower() == keyword.ToLower() && (x.LeaveDate == null && x.LeaveReason == null));

            if (beanE != null)
            {
                empBean.EmployeeNO = beanE.Account.Trim();
                empBean.EmployeeERPID = beanE.ErpId.Trim();
                empBean.EmployeeCName = beanE.Name2.Trim();
                empBean.EmployeeEName = beanE.Name.Trim();
                empBean.WorkPlace = beanE.WorkPlace.Trim();
                empBean.PhoneExt = beanE.Extension.Trim();
                empBean.CompanyCode = beanE.CompCde.Trim();
                empBean.BUKRS = getBUKRS(beanE.CompCde.Trim());
                empBean.EmployeeEmail = beanE.Email.Trim();
                empBean.EmployeePersonID = beanE.Id.Trim();

                var beanD = dbEIP.Departments.FirstOrDefault(x => x.Id == beanE.DeptId);

                if (beanD != null)
                {
                    empBean.DepartmentNO = beanD.Id.Trim();
                    empBean.DepartmentName = beanD.Name2.Trim();
                }
            }

            return empBean;
        }

        #region 取得SAP的公司別
        /// <summary>
        /// 取得SAP的公司別(T012、T016、C069、T022)
        /// </summary>
        /// <param name="tCompCode">公司別(Comp-1、Comp-2、Comp-3、Comp-4)</param>
        /// <returns></returns>
        public string getBUKRS(string tCompCode)
        {
            string reValue = "T012";

            switch (tCompCode.Trim())
            {
                case "Comp-1":
                    reValue = "T012";
                    break;
                case "Comp-2":
                    reValue = "T016";
                    break;
                case "Comp-3":
                    reValue = "C069";
                    break;
                case "Comp-4":
                    reValue = "T022";
                    break;
            }

            return reValue;
        }
        #endregion

        #region 人員資訊相關
        public struct EmployeeBean
        {
            /// <summary>人員帳號</summary>
            public string EmployeeNO { get; set; }
            /// <summary>ERPID</summary>
            public string EmployeeERPID { get; set; }
            /// <summary>中文姓名</summary>
            public string EmployeeCName { get; set; }
            /// <summary>英文姓名</summary>
            public string EmployeeEName { get; set; }
            /// <summary>工作地點</summary>
            public string WorkPlace { get; set; }
            /// <summary>分機</summary>
            public string PhoneExt { get; set; }
            /// <summary>公司別(Comp-1、Comp-2、Comp-3、Comp-4)</summary>
            public string CompanyCode { get; set; }
            /// <summary>工廠別(T012、T016、C069、T022_</summary>
            public string BUKRS { get; set; }
            /// <summary>部門代號</summary>
            public string DepartmentNO { get; set; }
            /// <summary>部門名稱</summary>
            public string DepartmentName { get; set; }
            /// <summary>人員Email</summary>
            public string EmployeeEmail { get; set; }
            /// <summary>人員ID(Person資料表ID)</summary>
            public string EmployeePersonID { get; set; }
        }
        #endregion

        #endregion

        #region 查詢客戶資料By公司別
        /// <summary>
        /// 查詢客戶資料By公司別
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="compcde">公司別</param>
        /// <returns></returns>
        public IQueryable<ViewCustomer2> findCustByKeywordAndComp(string keyword, string compcde)
        {
            return dbProxy.ViewCustomer2s.Where(x => x.KnvvVkorg == compcde &&
                                                   (x.Kna1Name1.Contains(keyword) || x.Kna1Kunnr.Contains(keyword))).Take(30);
        }
        #endregion

        #region 取得客戶聯絡人
        /// <summary>
        /// 取得客戶聯絡人
        /// </summary>
        /// <param name="cBUKRS">公司別(T012、T016、C069、T022)</param>
        /// <param name="CustomerID">客戶代號</param>        
        /// <returns></returns>
        public List<PCustomerContact> GetContactInfo(string cBUKRS, string CustomerID)
        {
            var qPjRec = dbProxy.CustomerContacts.OrderByDescending(x => x.ModifiedDate).
                                               Where(x => (x.Disabled == null || x.Disabled != 1) && x.ContactName != "" && x.ContactCity != "" &&
                                                          x.ContactAddress != "" && x.ContactPhone != "" &&
                                                          x.Knb1Bukrs == cBUKRS && x.Kna1Kunnr == CustomerID).ToList();

            List<PCustomerContact> liPCContact = new List<PCustomerContact>();
            if (qPjRec != null && qPjRec.Count() > 0)
            {
                foreach (var prBean in qPjRec)
                {
                    PCustomerContact prDocBean = new PCustomerContact();

                    prDocBean.ContactID = prBean.ContactId.ToString();
                    prDocBean.CustomerID = prBean.Kna1Kunnr.Trim().Replace(" ", "");
                    prDocBean.CustomerName = prBean.Kna1Name1.Trim().Replace(" ", "");
                    prDocBean.BUKRS = cBUKRS;
                    prDocBean.Name = prBean.ContactName.Trim().Replace(" ", "");
                    prDocBean.City = prBean.ContactCity.Trim().Replace(" ", "");
                    prDocBean.Address = prBean.ContactAddress.Trim().Replace(" ", "");
                    prDocBean.Email = prBean.ContactEmail.Trim().Replace(" ", "");
                    prDocBean.Phone = prBean.ContactPhone.Trim().Replace(" ", "");
                    prDocBean.BPMNo = prBean.BpmNo.Trim().Replace(" ", "");

                    liPCContact.Add(prDocBean);
                }
            }

            return liPCContact;
        }

        /// <summary>客戶聯絡人</summary>
        public struct PCustomerContact
        {
            /// <summary>GUID</summary>
            public string ContactID { get; set; }

            /// <summary>客戶ID</summary>
            public string CustomerID { get; set; }

            /// <summary>客戶名稱</summary>
            public string CustomerName { get; set; }

            /// <summary>公司別</summary>
            public string BUKRS { get; set; }

            /// <summary>聯絡人姓名</summary>
            public string Name { get; set; }

            /// <summary>聯絡人居住城市</summary>
            public string City { get; set; }

            /// <summary>聯絡人地址</summary>
            public string Address { get; set; }

            /// <summary>聯絡人Email</summary>
            public string Email { get; set; }

            /// <summary>聯絡人電話</summary>
            public string Phone { get; set; }

            /// <summary>來源表單</summary>
            public string BPMNo { get; set; }
        }
        #endregion

        #region 判斷登入者是否為MIS
        /// <summary>
        /// 判斷登入者是否為MIS
        /// </summary>
        /// <param name="LoginAccount">登入者帳號</param>
        /// <param name="tSysOperationID">程式作業編號檔系統ID(ALL，固定的GUID)</param>
        /// <returns></returns>
        public bool getIsMIS(string LoginAccount, string tSysOperationID)
        {
            bool reValue = false;

            Guid tcID = new Guid(tSysOperationID); //全模組

            var bean = psipDb.TbOneSysParameters.FirstOrDefault(x => x.COperationId == tcID && x.CFunctionId == "ACCOUNT" && x.CCompanyId == "ALL" && x.CNo == "MIS");

            if (bean != null)
            {
                if ( bean.CValue.ToLower() == LoginAccount.ToLower())
                {
                    reValue = true;                    
                }
            }

            return reValue;
        }
        #endregion

        #endregion -----↑↑↑↑↑共用方法 ↑↑↑↑↑-----
    }
}
