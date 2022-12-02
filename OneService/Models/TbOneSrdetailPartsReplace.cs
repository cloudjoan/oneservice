using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrdetailPartsReplace
    {
        public int CId { get; set; }
        public string CSrid { get; set; } = null!;
        public string? CXchp { get; set; }
        public string? CMaterialId { get; set; }
        public string? CMaterialName { get; set; }
        public string? COldCt { get; set; }
        public string? CNewCt { get; set; }
        public string? CHpct { get; set; }
        public string? CNewUefi { get; set; }
        public string? CStandbySerialId { get; set; }
        public string? CHpcaseId { get; set; }
        public DateTime? CArriveDate { get; set; }
        public DateTime? CReturnDate { get; set; }
        public string? CMaterialItem { get; set; }
        public string? CNote { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
