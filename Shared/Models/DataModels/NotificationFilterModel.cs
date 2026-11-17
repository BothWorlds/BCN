using System;

namespace BCN.Shared.Models.DataModels
{
    public class NotificationFilterModel
    {
        public string SearchTerm { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}