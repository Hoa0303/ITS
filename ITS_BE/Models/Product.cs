namespace ITS_BE.Models
{
    public class Product : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Discount { get; set; }
        public bool Enable { get; set; }
        public int Sold { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public Product_Details? Details { get; set; }
        public ICollection<Image> Images { get; } = new HashSet<Image>();
        public ICollection<Product_Color> Product_Colors { get; } = new HashSet<Product_Color>();
        public ICollection<OrderDetail> OrderDetails { get; } = new HashSet<OrderDetail>();
        public ICollection<ReceiptDetail> ReceiptDetails { get; } = new HashSet<ReceiptDetail>();
        public ICollection<Favorite> Favorites { get; } = new HashSet<Favorite>();
    }
}
