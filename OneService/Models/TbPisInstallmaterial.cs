using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbPisInstallmaterial
    {
        public string Srid { get; set; } = null!;
        public string? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public string Srserial { get; set; } = null!;
    }
}
