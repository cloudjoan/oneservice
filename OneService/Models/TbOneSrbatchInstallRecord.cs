using System;
using System.Collections.Generic;

namespace OneService.Models
{
    public partial class TbOneSrbatchInstallRecord
    {
        public Guid CGuid { get; set; }
        public string CSrid { get; set; } = null!;
        public string? CCustomerId { get; set; }
        public string? CCustomerName { get; set; }
        public string? CTeamId { get; set; }
        public string? CContactName { get; set; }
        public string? CContactAddress { get; set; }
        public string? CContactPhone { get; set; }
        public string? CContactMobile { get; set; }
        public string? CContactEmail { get; set; }
        public string? CMainEngineerId { get; set; }
        public string? CMainEngineerName { get; set; }
        public string? CSalesId { get; set; }
        public string? CSalesName { get; set; }
        public string? CSecretaryId { get; set; }
        public string? CSecretaryName { get; set; }
        public string? CSalesNo { get; set; }
        public string? CShipmentNo { get; set; }
        public string? CSerialId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserName { get; set; }
    }
}
