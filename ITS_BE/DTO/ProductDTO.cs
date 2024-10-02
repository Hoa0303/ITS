namespace ITS_BE.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Discount { get; set; }
        public bool Enable { get; set; }
        public double Price { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public float SizeScreen { get; set; }
        public int Ram { get; set; }
        public int Rom { get; set; }
        public string Cpu { get; set; }
        public string ImageUrl { get; set; }
    }
}
