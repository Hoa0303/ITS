using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;

namespace ITS_BE.Repository.UserRepository
{
    public class UserRepository(MyDbContext dbContext)
        : CommonRepository<User>(dbContext), IUserRepository
    {
    }
}
