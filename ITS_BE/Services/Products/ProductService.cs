using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.ImageRepository;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Repository.ProductDetailRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using ITS_BE.Storage;
using Microsoft.IdentityModel.Tokens;

namespace ITS_BE.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IProductColorRepository _productColorRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorage _fileStorage;
        private readonly string path = "assets/images/products";
        private readonly string path1 = "assets/images/colors";

        public ProductService(IProductRepository productRepository, IImageRepository imageRepository, IMapper mapper, IFileStorage fileStorage,
            IProductColorRepository productColorRepository, IProductDetailRepository productDetailRepository)
        {
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _productDetailRepository = productDetailRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
            _fileStorage = fileStorage;
        }

        public async Task<ProductDTO> CreateProduct(ProductRequest request, IFormFileCollection images)
        {
            try
            {
                var product = _mapper.Map<Product>(request);

                await _productRepository.AddAsync(product);

                IList<string> fileNames = new List<string>();
                var imgs = images.Select(file =>
                {
                    var name = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    fileNames.Add(name);
                    var image = new Image()
                    {
                        ProductId = product.Id,
                        ImageUrl = Path.Combine(path, name)
                    };
                    return image;
                });
                await _imageRepository.AddAsync(imgs);
                await _fileStorage.SaveAsync(path, images, fileNames);

                IList<string> imgColors = new List<string>();
                IList<IFormFile> files = new List<IFormFile>();
                //var productColors = request.Color.Select(color =>
                //{
                //    var name = Guid.NewGuid().ToString() + Path.GetExtension(color.File.FileName);
                //    imgColors.Add(name);
                //    files.Add(color.File);
                //    var productcolor = new Product_Color()
                //    {
                //        ProductId = product.Id,
                //        ColorId = color.ColorId,
                //        Prices = color.Prices,
                //        Quantity = color.Quantity,
                //        ImageUrl = Path.Combine(path1, name)
                //    };
                //    return productcolor;
                //});
                //await _productColorRepository.AddAsync(productColors);
                await _fileStorage.SaveAsync(path1, files, imgColors);

                //var productDetail = new Product_Details()
                //{
                //    SizeScreen = request.Details.SizeScreen,
                //    ScanHz = request.Details.ScanHz,
                //    Material = request.Details.Material,
                //    RearCam = request.Details.RearCam,
                //    FrontCam = request.Details.FrontCam,
                //    Cpu = request.Details.Cpu,
                //    Ram = request.Details.Ram,
                //    Rom = request.Details.Rom,
                //    Battery = request.Details.Battery,
                //    size = request.Details.size,
                //    weight = request.Details.weight,
                //    price = request.Details.price,
                //};
                var productDetail = _mapper.Map<Product_Details>(request.Details);
                productDetail.ProductId = product.Id;
                await _productDetailRepository.AddAsync(productDetail);

                var res = _mapper.Map<ProductDTO>(product);
                var image = await _imageRepository.GetFirstByProductAsync(product.Id);
                if (image != null)
                {
                    res.ImageUrl = image.ImageUrl;
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task Delete(int id)
        {
            var product = await _productRepository.FindAsync(id);
            if (product != null)
            {
                var images = await _imageRepository.GetImageProductAsync(id);
                _fileStorage.Delete(images.Select(image => image.ImageUrl));

                await _productRepository.DeleteAsync(product);
            }
            else throw new ArgumentException($"Id {id} " + ErrorMessage.NOT_FOUND);
        }

        public async Task<PageRespone<ProductDTO>> GetAllProduct(int page, int pageSize, string? search)
        {
            try
            {
                int totalProduct;
                IEnumerable<Product> products;
                if (string.IsNullOrEmpty(search))
                {
                    totalProduct = await _productRepository.CountAsync();
                    products = await _productRepository.GetPageProduct(page, pageSize);
                }
                else
                {
                    totalProduct = await _productRepository.CountAsync(search);
                    products = await _productRepository.GetPageProduct(page, pageSize, search);
                }
                var res = _mapper.Map<IEnumerable<ProductDTO>>(products);
                foreach (var product in res)
                {
                    var image = await _imageRepository.GetFirstByProductAsync(product.Id);
                    if (image != null)
                    {
                        product.ImageUrl = image.ImageUrl;
                    }
                }
                return new PageRespone<ProductDTO>
                {
                    Items = res,
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalProduct
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<ProductDetailRespone> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product != null)
            {
                var res = _mapper.Map<ProductDetailRespone>(product);
                res.ImageUrls = product.Images.Select(x => x.ImageUrl);
                var productColors = await _productColorRepository.GetColorProductAsync(product.Id);
                res.Color = _mapper.Map<IEnumerable<ColorResponse>>(productColors);
                return res;
            }
            else throw new ArgumentException($"Id {id}" + ErrorMessage.NOT_FOUND);
        }

        public async Task<ProductDTO> UpdateProduct(int id, ProductRequest request, IFormFileCollection images)
        {
            var product = await _productRepository.SingleOrDefaultAsync(id);
            var productDetail = await _productDetailRepository.GetDetailProductAsync(id);
            if (product != null)
            {
                try
                {
                    product.Name = request.Name;
                    product.Discount = request.Discount;
                    product.BrandId = request.BrandId;
                    product.CategoryId = request.CategoryId;
                    product.Details.SizeScreen = request.Details.SizeScreen;
                    product.Details.ScanHz = request.Details.ScanHz;
                    product.Details.Material = request.Details.Material;
                    product.Details.RearCam = request.Details.RearCam;
                    product.Details.FrontCam = request.Details.FrontCam;
                    product.Details.Cpu = request.Details.Cpu;
                    product.Details.Ram = request.Details.Ram;
                    product.Details.Rom = request.Details.Rom;
                    product.Details.Battery = request.Details.Battery;
                    product.Details.size = request.Details.size;
                    product.Details.weight = request.Details.weight;

                    var productColor = await _productColorRepository.GetColorProductAsync(id);
                    List<Product_Color> colorDelete = new();
                    if (productColor.IsNullOrEmpty())
                    {
                        colorDelete.AddRange(productColor);
                    }
                    else
                    {
                        //var clDelete = productColor.Where(old => !request.Color.Contains(old.Color));
                    }

                    //var oldImg = await _imageRepository.GetImageProductAsync(id);
                    var oldImg = product.Images;

                    List<Image> imageDelete = new();
                    if (request.ImageUrls.IsNullOrEmpty())
                    {
                        imageDelete.AddRange(oldImg);
                    }
                    else
                    {
                        var imgDelete = oldImg.Where(old => !request.ImageUrls.Contains(old.ImageUrl));
                        imageDelete.AddRange(imgDelete);
                    }
                    _fileStorage.Delete(imageDelete.Select(e => e.ImageUrl));
                    await _imageRepository.DeleteAsync(imageDelete);

                    if (images.Count > 0)
                    {
                        List<string> fileNames = new();
                        var imgs = images.Select(file =>
                        {
                            var name = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            fileNames.Add(name);
                            var image = new Image()
                            {
                                ProductId = product.Id,
                                ImageUrl = Path.Combine(path, name)
                            };
                            return image;
                        });
                        await _imageRepository.AddAsync(imgs);
                        await _fileStorage.SaveAsync(path, images, fileNames);
                    }
                    await _productRepository.UpdateAsync(product);
                    return _mapper.Map<ProductDTO>(product);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.InnerException?.Message ?? ex.Message);
                }
            }
            else throw new ArgumentException($"Id {id} " + ErrorMessage.NOT_FOUND);
        }
    }
}
