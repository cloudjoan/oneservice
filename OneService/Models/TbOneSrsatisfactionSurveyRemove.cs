using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrsatisfactionSurveyRemove
    {
        public int CId { get; set; }
        public string? CDimension { get; set; }
        public string? CCustomerId { get; set; }
        public string? CCustomerName { get; set; }
        public string? CContactEmail { get; set; }
        public int? Disabled { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedUserName { get; set; }
    }
}
