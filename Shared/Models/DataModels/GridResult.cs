using System.Collections.Generic;

namespace BCN.Shared.Models.DataModels
{
    public class GridResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
    }
}