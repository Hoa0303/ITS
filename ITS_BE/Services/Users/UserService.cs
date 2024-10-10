using AutoMapper;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.UserRepository;
using ITS_BE.Response;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace ITS_BE.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IDeliveryAddressRepository _deliveryAddressRepository;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, IUserRepository userRepository, IDeliveryAddressRepository deliveryAddressRepository, IMapper mapper)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _deliveryAddressRepository = deliveryAddressRepository;
            _mapper = mapper;
        }

        public async Task<PageRespone<UserResponse>> GetAllAsync(int page, int pageSize, string? key)
        {
            int totalUser;
            IEnumerable<User> users;
            if (string.IsNullOrEmpty(key))
            {
                totalUser = await _userRepository.CountAsync();
                users = await _userRepository.GetPagedOrderByDescendingAsync(page, pageSize, null, e => e.CreateAt);
            }
            else
            {
                Expression<Func<User, bool>> expression = e => e.Id.Contains(key)
                    || (e.FullName != null && e.FullName.Contains(key))
                    || (e.Email != null && e.Email.Contains(key))
                    || (e.PhoneNumber != null && e.PhoneNumber.Contains(key));

                totalUser = await _userRepository.CountAsync(expression);
                users = await _userRepository.GetPagedOrderByDescendingAsync(page, pageSize, null, e => e.CreateAt);
            }

            var items = _mapper.Map<IEnumerable<UserResponse>>(users);

            return new PageRespone<UserResponse>
            {
                Items = items,
                TotalItems = totalUser,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task<AddressDTO?> GetUserAddress(string userId)
        {
            var delivery = await _deliveryAddressRepository.SingleOrDefaultAsync(e => e.UserId == userId);
            if (delivery != null)
            {
                return _mapper.Map<AddressDTO>(delivery);
            }
            return null;
        }

        public async Task<AddressDTO?> UpdateOrAddUserAddress(string userId, AddressDTO addressDTO)
        {
            var delivery = await _deliveryAddressRepository.SingleOrDefaultAsync(e => e.UserId == userId);
            if (delivery == null)
            {
                delivery = new DeliveryAddress { UserId = userId, Name = addressDTO.Name };
                await _deliveryAddressRepository.AddAsync(delivery);
            }

            delivery.Province_code = addressDTO.Province_code;
            delivery.Province_name = addressDTO.Province_name;
            delivery.District_code = addressDTO.District_code;
            delivery.District_name = addressDTO.District_name;
            delivery.Ward_code = addressDTO.Ward_code;
            delivery.Ward_name = addressDTO.Ward_name;
            delivery.Detail = addressDTO.Detail;
            delivery.Name = addressDTO.Name;
            delivery.PhoneNumber = addressDTO.PhoneNumber;

            await _deliveryAddressRepository.UpdateAsync(delivery);
            return _mapper.Map<AddressDTO?>(delivery);
        }
    }
}
