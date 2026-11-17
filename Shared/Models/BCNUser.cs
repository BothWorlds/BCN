using System;
using System.Collections.Generic;
using Oqtane.Models;

namespace BCN.Shared.Models
{
    public class BCNUser : IAuditable, IDeletable
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string TimeZoneId { get; set; }

        public int? PhotoFileId { get; set; }

        public DateTime? LastLoginOn { get; set; }

        public string LastIPAddress { get; set; }

        public bool TwoFactorRequired { get; set; }

        public string TwoFactorCode { get; set; }

        public DateTime? TwoFactorExpiry { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}