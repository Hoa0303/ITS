namespace ITS_BE.DTO
{
    public class ReviewDTO
    {
        public string Id { get; set; }
        public string? Description { get; set; }
        public int Start {  get; set; }
        public string FullName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime createAt { get; set; }
    }
}
