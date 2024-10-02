using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using System.Linq.Expressions;

namespace ITS_BE.Repository.CartItemRepository
{
    public interface ICartItemRepository : ICommonRepository<CartItem>
    {
        Task<CartItem?> SingleOrDefaultAsyncInclude(Expression<Func<CartItem, bool>> expression);
    }
}
