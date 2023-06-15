using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbAccessHistory
    {
        public int Id { get; set; }
        public string? ErpId { get; set; }
        public string? UserName { get; set; }
        public string? AccessLocation { get; set; }
        public string? AccessResulst { get; set; }
        public string? InsertTime { get; set; }
    }
}
