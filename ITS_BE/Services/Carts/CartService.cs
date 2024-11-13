using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.Models;
using ITS_BE.Repository.CartItemRepository;
using ITS_BE.Repository.ImageRepository;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Request;
using ITS_BE.Response;

namespace ITS_BE.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IProductColorRepository _productColorRepository;
        private readonly IMapper _mapper;

        public CartService(ICartItemRepository cartItemRepository, IImageRepository imageRepository, IProductColorRepository productColorRepository, IMapper mapper)
        {
            _cartItemRepository = cartItemRepository;
            _imageRepository = imageRepository;
            _productColorRepository = productColorRepository;
            _mapper = mapper;
        }

        public async Task AddToCart(string userId, CartRequest request)
        {
            try
            {
                var color = await _productColorRepository
                    .SingleAsync(e => e.ColorId == request.ColorId && e.ProductId == request.ProductId);
                if (color.Quantity <= 0)
                {
                    throw new InvalidDataException(ErrorMessage.SOLDOUT);
                }

                var exist = await _cartItemRepository.SingleOrDefaultAsync(
                    e => e.ProductId == request.ProductId &&
                         e.UserId == userId &&
                         e.ColorId == request.ColorId);

                if (exist != null)
                {
                    if ((request.Quantity + exist.Quantity) > color.Quantity)
                    {
                        throw new InvalidDataException(ErrorMessage.CART_MAXIMUM);
                    }
                    exist.Quantity += request.Quantity;
                    await _cartItemRepository.UpdateAsync(exist);
                }
                else
                {
                    CartItem item = new()
                    {
                        ProductId = request.ProductId,
                        UserId = userId,
                        ColorId = request.ColorId,
                        Quantity = request.Quantity,
                    };
                    await _cartItemRepository.AddAsync(item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteCartItem(string cartId, string userId)
        {
            var cartItem = await _cartItemRepository.SingleOrDefaultAsync(e => e.Id == cartId && e.UserId == userId);
            if (cartItem != null)
            {
                await _cartItemRepository.DeleteAsync(cartItem);
            }
            else throw new ArgumentException($"Id {cartId} " + ErrorMessage.NOT_FOUND);
        }

        public async Task<IEnumerable<CartItemResponse>> GetAllByUserId(string userId)
        {
            var items = await _cartItemRepository.GetAsync(e => e.UserId == userId);
            var res = items.Select(cartItem =>
            {
                var color = cartItem.Product.Product_Colors.Single(x => x.ColorId == cartItem.ColorId);

                return new CartItemResponse
                {
                    Id = cartItem.Id,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.Name,
                    CategoryName = cartItem.Product.Category.Name,
                    Rom = cartItem.Product.Details.Rom,
                    OriginPrice = color.Prices,
                    Discount = cartItem.Product.Discount,
                    Quantity = cartItem.Quantity,
                    ImageUrl = color.ImageUrl,
                    ColorId = cartItem.ColorId,
                    ColorName = color.Color.Name,
                    InStock = color.Quantity,
                };
            });
            return res;
        }

        public async Task<IEnumerable<int>> GetCountProductId(string userId)
        {
            var cart = await _cartItemRepository.GetAsync(e => e.UserId == userId);
            return cart.Select(e => e.ProductId);
        }

        public async Task<CartItemResponse> UpdateCartItem(string cartId, string userId, UpdateCartItemRequest request)
        {
            try
            {
                var cartItem = await _cartItemRepository.SingleOrDefaultAsyncInclude(e => e.Id == cartId && e.UserId == userId);
                if (cartItem != null)
                {
                    var color = cartItem.Product.Product_Colors.Single(x => x.ColorId == cartItem.ColorId);

                    if (request.Quantity.HasValue)
                    {
                        if (color.Quantity > 0 && request.Quantity.Value <= color.Quantity)
                        {
                            cartItem.Quantity = request.Quantity.Value;
                        }
                        else throw new Exception(ErrorMessage.SOLDOUT);
                    }
                    await _cartItemRepository.UpdateAsync(cartItem);

                    return new CartItemResponse
                    {
                        Id = cartItem.Id,
                        ProductId = cartItem.ProductId,
                        ProductName = cartItem.Product.Name,
                        CategoryName = cartItem.Product.Category.Name,
                        Rom = cartItem.Product.Details.Rom,
                        OriginPrice = color.Prices,
                        Discount = cartItem.Product.Discount,
                        Quantity = cartItem.Quantity,
                        ImageUrl = color.ImageUrl,
                        ColorId = cartItem.ColorId,
                        ColorName = color.Color.Name,
                        InStock = color.Quantity
                    };
                }
                throw new ArgumentException(ErrorMessage.NOT_FOUND + " product");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
