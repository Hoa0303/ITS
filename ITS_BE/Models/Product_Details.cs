namespace ITS_BE.Models
{
    public class Product_Details : IBaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
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
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
