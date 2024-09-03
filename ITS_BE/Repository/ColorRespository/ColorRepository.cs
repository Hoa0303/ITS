using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.ColorRespository
{
    public class ColorRepository : CommonRepository<Color>, IColorRepository
    {
        public ColorRepository(MyDbContext _contetx) : base(_contetx) { }

    }
}
