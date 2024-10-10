using ITS_BE.DTO;
using ITS_BE.Response;

namespace ITS_BE.Services.Users
{
    public interface IUserService
    {
        Task<PageRespone<UserResponse>> GetAllAsync(int page, int pageSize, string? key);
        Task<AddressDTO?> GetUserAddress(string userId);
        Task<AddressDTO?> UpdateOrAddUserAddress(string userId, AddressDTO addressDTO);
    }
}
