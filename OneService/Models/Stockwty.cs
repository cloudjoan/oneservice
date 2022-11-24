using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class Stockwty
    {
        public Guid CId { get; set; }
        public string IvSerial { get; set; } = null!;
        public string? IvSono { get; set; }
        public DateTime? IvDndate { get; set; }
        public string? IvCid { get; set; }
        public string? IvWtyid { get; set; }
        public string? IvWtydesc { get; set; }
        public DateTime? IvSdate { get; set; }
        public DateTime? IvEdate { get; set; }
        public string? IvSlasrv { get; set; }
        public string? IvSlaresp { get; set; }
        public string? BpNo { get; set; }
        public string? CarePackNo { get; set; }
        public string? Note { get; set; }
        public string? BpmNo { get; set; }
        public string? Advice { get; set; }
        public string? ReceiptNo { get; set; }
        public string? ReceiptDate { get; set; }
        public string? CrUser { get; set; }
        public DateTime? CrDate { get; set; }
        public string? UpUser { get; set; }
        public DateTime? UpDate { get; set; }
    }
}
