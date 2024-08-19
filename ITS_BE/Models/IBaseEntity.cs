namespace ITS_BE.Models
{
    public interface IBaseEntity
    {
        DateTime CreateAt { get; set; }
        DateTime? UpdateAt { get; set; }
    }
}
