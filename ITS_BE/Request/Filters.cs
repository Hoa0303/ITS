using ITS_BE.Enumerations;

namespace ITS_BE.Request
{
    public class Filters : PageResquest
    {
        public SortEnum Sorter { get; set; } = 0;
        public IEnumerable<int>? CategoryIds { get; set; }
        public IEnumerable<int>? BrandIds { get; set; }
        public IEnumerable<int>? Ram { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
    }
}
