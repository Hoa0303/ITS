namespace ITS_BE.Request
{
    public class IdRequest<TKey>
    {
        public TKey Id { get; set; }
    }
    public class NameRequest
    {
        public string Name { get; set; }
    }
}
