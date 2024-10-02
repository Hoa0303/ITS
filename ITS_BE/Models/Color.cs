namespace ITS_BE.Models
{
    public class Color : IBaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public ICollection<Product_Color> Product_Colors { get; } = new HashSet<Product_Color>();
    }
}
