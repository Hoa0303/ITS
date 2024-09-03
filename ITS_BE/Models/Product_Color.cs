namespace ITS_BE.Models
{
    public class Product_Color
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ColorId { get; set; }
        public Color Color { get; set; }
        public double Prices { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
