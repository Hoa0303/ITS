namespace ITS_BE.Request
{
    public class PageResquest
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string? search { get; set; }
    }
}
