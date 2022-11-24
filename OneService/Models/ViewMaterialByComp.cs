using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class ViewMaterialByComp
    {
        public string MaraMatnr { get; set; } = null!;
        public string? MaktTxza1Zf { get; set; }
        public string? MaraStxl1 { get; set; }
        public string? MvkeProdh { get; set; }
        public string? MaraMatkl { get; set; }
        public string MardWerks { get; set; } = null!;
        public string MvkeVkorg { get; set; } = null!;
    }
}
