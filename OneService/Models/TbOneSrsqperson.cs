using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrsqperson
    {
        public int CId { get; set; }
        public string? CFirstKey { get; set; }
        public string? CSecondKey { get; set; }
        public string? CThirdKey { get; set; }
        public string? CNo { get; set; }
        public string? CContent { get; set; }
        public string? CEngineerId { get; set; }
        public string? CEngineerName { get; set; }
        public string? CFullKey { get; set; }
        public string? CFullName { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
