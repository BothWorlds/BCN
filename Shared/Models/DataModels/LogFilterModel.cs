using System;

namespace BCN.Shared.Models.DataModels
{
    public class LogFilterModel
    {
        public string Level { get; set; }
        public string Feature { get; set; }
        public string Function { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? LogDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}