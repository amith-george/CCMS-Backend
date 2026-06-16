using System.Collections.Generic;

namespace CCMS.Application.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalPages => (TotalCount + Limit - 1) / Limit;
    }
}
