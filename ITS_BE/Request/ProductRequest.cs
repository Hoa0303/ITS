namespace ITS_BE.Request
{

    public class ImageColor
    {
        public int ColorId { get; set; }
        public double Prices { get; set; }
        public int Quantity { get; set; }
        public IFormFile? File { get; set; }
    }

    public class Details
    {
        public float SizeScreen { get; set; }
        public string ScanHz { get; set; }
        public string Material { get; set; }
        public string RearCam { get; set; }
        public string FrontCam { get; set; }
        public string Cpu { get; set; }
        public int Ram { get; set; }
        public int Rom { get; set; }
        public string Battery { get; set; }
        public string size { get; set; }
        public float weight { get; set; }
    }
    public class ProductRequest
    {
        public string Name { get; set; }
        public int Discount {  get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<ImageColor> Color { get; set; }
        public Details Details { get; set; }
        public IEnumerable<string> ImageUrls { get; set; } = new List<string>();
    }
}
