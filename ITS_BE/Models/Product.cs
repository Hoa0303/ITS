namespace ITS_BE.Models
{
    public class Product : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Discount {  get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public ICollection<Image> Images { get; set; } = new HashSet<Image>();
        public Product_Details? Details { get; set; }
    }
}
