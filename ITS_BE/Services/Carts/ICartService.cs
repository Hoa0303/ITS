using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Carts
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemResponse>> GetAllByUserId(string userId);
        Task<IEnumerable<int>> GetCountProductId(string userId);
        Task AddToCart(string userId, CartRequest request);
        Task<CartItemResponse> UpdateCartItem(string cartId, string userId, UpdateCartItemRequest request);
        Task DeleteCartItem(string cartId, string userId);
    }
}
