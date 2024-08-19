using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.CategoryRepository
{
    public class CaterogyRepository : CommonRepository<Category>, ICateroryRepository
    {
        public CaterogyRepository(MyDbContext context) : base(context) { }
    }
}
