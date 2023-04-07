using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class MartAnalyseServiceRequestLabor
    {
        public string? Srid { get; set; }
        public string? Description { get; set; }
        public string? ProcessTypeText { get; set; }
        public string? StatusText { get; set; }
        public string? Customer { get; set; }
        public string? EngineerId { get; set; }
        public string? Engineer { get; set; }
        public string? Contact { get; set; }
        public string? Finish { get; set; }
        public int? Labor { get; set; }
        public string? Pk { get; set; }
    }
}
