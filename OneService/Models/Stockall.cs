using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class Stockall
    {
        public string IvSerial { get; set; } = null!;
        public string? IvVendor { get; set; }
        public string? IvVname { get; set; }
        public string? IvPono { get; set; }
        public DateTime? IvGrdate { get; set; }
        public string ProdId { get; set; } = null!;
        public string? Product { get; set; }
        public string? ProdHierarchy { get; set; }
        public string? SoNo { get; set; }
        public decimal? SoAmount { get; set; }
        public string? SoSales { get; set; }
        public string? SoSalesName { get; set; }
        public string? Internalno { get; set; }
        public DateTime? IvDndate { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? IvSlasrv { get; set; }
        public string? IvSlaresp { get; set; }
        public string? OppNo { get; set; }
        public string? ProjName { get; set; }
        public string? VendoromWtyid { get; set; }
        public DateTime? VendoromSdate { get; set; }
        public DateTime? VendoromEdate { get; set; }
        public string? OmWtyid { get; set; }
        public DateTime? OmSdate { get; set; }
        public DateTime? OmEdate { get; set; }
        public string? ExWtyid { get; set; }
        public DateTime? ExSdate { get; set; }
        public DateTime? ExEdate { get; set; }
        public string? TmWtyid { get; set; }
        public DateTime? TmSdate { get; set; }
        public DateTime? TmEdate { get; set; }
        public string? ContractId { get; set; }
        public string? ContractName { get; set; }
        public DateTime? ContractSdate { get; set; }
        public DateTime? ContractEdate { get; set; }
        public string? RenewStatus { get; set; }
        public DateTime? CrDate { get; set; }
    }
}
