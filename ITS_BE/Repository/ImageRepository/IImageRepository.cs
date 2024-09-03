using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.ImageRepository
{
    public interface IImageRepository : ICommonRepository<Image>
    {
        Task<Image?> GetFirstByProductAsync(int id);
        Task<IEnumerable<Image>> GetImageProductAsync(int ProductId);
    }
}
