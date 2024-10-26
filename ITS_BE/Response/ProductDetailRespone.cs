namespace ITS_BE.Response
{
    public class ColorResponse
    {
        public int ColorId { get; set; }
        public double Prices { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
    public class ProductDetailRespone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Discount { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public float Rating { get; set; }
        public int RatingCount {  get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public float SizeScreen { get; set; }
        public string ScanHz { get; set; }
        public string Material { get; set; }
        public string RearCam { get; set; }
        public string FrontCam { get; set; }
        public string Cpu { get; set; }
        public string Gpu { get; set; }
        public int Ram { get; set; }
        public int Rom { get; set; }
        public string Battery { get; set; }
        public string size { get; set; }
        public float weight { get; set; }
        public string version { get; set; }
        public string line { get; set; }
        public IEnumerable<ColorResponse> Color { get; set; }
        public IEnumerable<string> ImageUrls { get; set; }
    }
}
