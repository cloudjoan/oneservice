using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrteamMapping
    {
        public int CId { get; set; }
        public string CTeamNewId { get; set; } = null!;
        public string? CTeamNewName { get; set; }
        public string CTeamOldId { get; set; } = null!;
        public string? CTeamOldName { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
