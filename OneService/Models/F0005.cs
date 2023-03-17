using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class F0005
    {
        public int Id { get; set; }
        public string Modt { get; set; } = null!;
        public string Alias { get; set; } = null!;
        public string Codet { get; set; } = null!;
        public string Codets { get; set; } = null!;
        public string? Dsc1 { get; set; }
        public string? Dsc2 { get; set; }
        public string? Rmk { get; set; }
        public int? Coso { get; set; }
        public string? Re1 { get; set; }
        public string? Re2 { get; set; }
        public int? Re3 { get; set; }
        public int? Re4 { get; set; }
        public DateTime? Re5 { get; set; }
        public DateTime? Re6 { get; set; }
        public string? CrUser { get; set; }
        public DateTime? CrDate { get; set; }
        public string? ApUser { get; set; }
        public DateTime? ApDate { get; set; }
        public string? UpUser { get; set; }
        public DateTime? UpDate { get; set; }
    }
}
