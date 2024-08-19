using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.BrandRepository
{
    public class BrandRepository : CommonRepository<Brand>, IBrandRepository
    {
        public BrandRepository(MyDbContext context) : base(context)
        {
        }
    }
}
