using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.FavoriteRepository;
using ITS_BE.Repository.ImageRepository;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Repository.ProductDetailRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Repository.UserRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using ITS_BE.Storage;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace ITS_BE.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IFavoriterRepository _favoriterRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductColorRepository _productColorRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IDeliveryAddressRepository _deliveryAddressRepository;
        private readonly IFileStorage _fileStorage;
        private readonly IMapper _mapper;
        private readonly string path = "assets/images/avatar";

        public UserService(UserManager<User> userManager, IUserRepository userRepository,
            IDeliveryAddressRepository deliveryAddressRepository,
            IMapper mapper, IFavoriterRepository favoriterRepository,
            IProductRepository productRepository, IProductColorRepository productColorRepository,
            IProductDetailRepository productDetailRepository, IImageRepository imageRepository,
            IFileStorage fileStorage)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _deliveryAddressRepository = deliveryAddressRepository;
            _favoriterRepository = favoriterRepository;
            _fileStorage = fileStorage;
            _mapper = mapper;
            _productDetailRepository = productDetailRepository;
            _imageRepository = imageRepository;
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
                users = await _userRepository.GetPagedOrderByDescendingAsync(page, pageSize, expression, e => e.CreateAt);
            }

            var items = _mapper.Map<IEnumerable<UserResponse>>(users).Select(e =>
            {
                e.LockedOut = e.LockoutEnd > DateTime.Now;
                e.LockoutEnd = e.LockoutEnd > DateTime.Now ? e.LockoutEnd : null;
                return e;
            });

            return new PageRespone<UserResponse>
            {
                Items = items,
                TotalItems = totalUser,
                Page = page,
                PageSize = pageSize,
            };
        }

        public async Task LockOut(string id, DateTimeOffset? endDate)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (endDate != null)
                    user.LockoutEnd = endDate.Value.AddDays(1);
                else user.LockoutEnd = endDate;

                await _userManager.UpdateAsync(user);
            }
            else throw new ArgumentException($"Id {id} " + ErrorMessage.NOT_FOUND);
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

        private string MaskEmail(string email)
        {
            var emailParts = email.Split('@');
            if (emailParts.Length != 2)
            {
                throw new ArgumentException("Email không hợp lệ");
            }

            string name = emailParts[0];
            string domain = emailParts[1];

            int visibleChars = name.Length < 5 ? 2 : 5;
            string maskedName = name[..visibleChars].PadRight(name.Length, '*');

            return $"{maskedName}@{domain}";
        }

        public async Task<UserDTO> GetUserInfo(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var res = _mapper.Map<UserDTO>(user);
                res.Email = res.Email != null ? MaskEmail(res.Email) : "";
                return res;
            }
            throw new InvalidOperationException(ErrorMessage.NOT_FOUND);
        }

        public async Task<UserInfoRequest> UpdateUserInfo(string userId, UserInfoRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                await _userManager.UpdateAsync(user);

                return _mapper.Map<UserInfoRequest>(user);
            }
            throw new InvalidOperationException(ErrorMessage.NOT_FOUND);
        }

        public async Task AddFavorite(string userId, int productId)
        {
            var favorite = new Favorite
            {
                UserId = userId,
                ProductId = productId
            };
            await _favoriterRepository.AddAsync(favorite);
        }

        public async Task<IEnumerable<int>> GetFavorites(string userId)
        {
            var favorite = await _favoriterRepository.GetAsync(e => e.UserId == userId);
            return favorite.Select(e => e.ProductId);
        }

        public async Task DeleteFavorite(string userId, int productId)
            => await _favoriterRepository.DeleteAsync(userId, productId);

        public async Task<PageRespone<ProductDTO>> GetProductFavorites(string userId, PageResquest request)
        {
            var favorites = await _favoriterRepository
                .GetPagedAsync(request.page, request.pageSize, e => e.UserId == userId, e => e.CreateAt);

            var total = await _favoriterRepository.CountAsync(e => e.UserId == userId);
            var productIds = favorites.Select(e => e.ProductId).ToList();

            var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));

            var items = _mapper.Map<IEnumerable<ProductDTO>>(products).Select(x =>
            {
                var image = products.Single(e => e.Id == x.Id).Images.First();
                if (image != null)
                {
                    x.ImageUrl = image.ImageUrl;
                }
                var color = products.Single(e => e.Id == x.Id).Product_Colors.First();
                if (color != null)
                {
                    x.Price = color.Prices;
                    x.ColorId = color.ColorId;
                }
                var detail = products.Single(e => e.Id == x.Id).Details;
                if (detail != null)
                {
                    x.SizeScreen = detail.SizeScreen;
                    x.Cpu = detail.Cpu;
                    x.Ram = detail.Ram;
                    x.Rom = detail.Rom;
                }
                return x;
            });
            return new PageRespone<ProductDTO>
            {
                Items = items,
                Page = request.page,
                PageSize = request.pageSize,
                TotalItems = total
            };
        }

        public async Task<UserDTO> UpdateImage(string userId, IFormFile img)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (user.ImageUrl != null)
                {
                    _fileStorage.Delete(user.ImageUrl);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                user.ImageUrl = Path.Combine(path, fileName);
                await _fileStorage.SaveAsync(path, img, fileName);

                await _userManager.UpdateAsync(user);
                return _mapper.Map<UserDTO>(user);
            }
            else { throw new ArgumentException(ErrorMessage.NOT_FOUND); }
        }

        public async Task<ImageDTO> GetImage(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return _mapper.Map<ImageDTO>(user);
            }
            else { throw new ArgumentException(ErrorMessage.NOT_FOUND); }
        }
    }
}
