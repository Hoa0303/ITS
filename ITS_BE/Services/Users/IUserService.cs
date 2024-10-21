using ITS_BE.DTO;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Users
{
    public interface IUserService
    {
        Task<PageRespone<UserResponse>> GetAllAsync(int page, int pageSize, string? key);
        Task<UserDTO> GetUserInfo(string userId);
        Task<UserInfoRequest> UpdateUserInfo(string userId, UserInfoRequest request);
        Task<AddressDTO?> GetUserAddress(string userId);
        Task<AddressDTO?> UpdateOrAddUserAddress(string userId, AddressDTO addressDTO);
        Task LockOut(string id, DateTimeOffset? endDate);

        Task AddFavorite(string userId, int productId);
        Task<IEnumerable<int>> GetFavorites(string userId);
        Task<PageRespone<ProductDTO>> GetProductFavorites(string userId, PageResquest request);
        Task DeleteFavorite(string userId, int productId);
    }
}
