using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using Microsoft.EntityFrameworkCore;

namespace ITS_BE.Repository.ImageRepository
{
    public class ImageRepository : CommonRepository<Image>, IImageRepository
    {
        private readonly MyDbContext _context;
        public ImageRepository(MyDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Image?> GetFirstByProductAsync(int id)
            => await _context.Images.FirstOrDefaultAsync(x => x.ProductId == id);

        public async Task<IEnumerable<Image>> GetImageProductAsync(int ProductId)
            => await _context.Images.Where(e => e.ProductId == ProductId).ToListAsync();
    }
}
