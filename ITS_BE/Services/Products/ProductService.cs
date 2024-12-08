using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Enumerations;
using ITS_BE.Models;
using ITS_BE.Repository.ImageRepository;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Repository.ProductDetailRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Repository.ReviewRepository;
using ITS_BE.Request;
using ITS_BE.Response;
using ITS_BE.Storage;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace ITS_BE.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IProductColorRepository _productColorRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorage _fileStorage;
        private readonly string path = "assets/images/products";
        private readonly string path1 = "assets/images/colors";

        public ProductService(IProductRepository productRepository, IImageRepository imageRepository,
            IMapper mapper, IFileStorage fileStorage, IProductColorRepository productColorRepository,
            IProductDetailRepository productDetailRepository, IReviewRepository reviewRepository)
        {
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _productDetailRepository = productDetailRepository;
            _reviewRepository = reviewRepository;
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
                var productColors = request.Color.Select(color =>
                {
                    var name = Guid.NewGuid().ToString() + Path.GetExtension(color.File.FileName);
                    imgColors.Add(name);
                    files.Add(color.File);
                    var productcolor = new Product_Color()
                    {
                        ProductId = product.Id,
                        ColorId = color.ColorId,
                        Prices = color.Prices,
                        Quantity = color.Quantity,
                        ImageUrl = Path.Combine(path1, name)
                    };
                    return productcolor;
                });
                await _productColorRepository.AddAsync(productColors);
                await _fileStorage.SaveAsync(path1, files, imgColors);

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
                //var images = await _imageRepository.GetImageProductAsync(id);
                //_fileStorage.Delete(images.Select(image => image.ImageUrl));

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
                    var color = await _productColorRepository.GetFirstColorByProductAsync(product.Id);
                    if (color != null)
                    {
                        product.Price = color.Prices;
                    }
                    var detail = (await _productDetailRepository.GetDetailProductAsync(product.Id)).FirstOrDefault();
                    if (detail != null)
                    {
                        product.SizeScreen = detail.SizeScreen;
                        product.Ram = detail.Ram;
                        product.Rom = detail.Rom;
                        product.Cpu = detail.Cpu;
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

        public async Task<PageRespone<ProductDTO>> OrderByDescendingBySold(int page, int pageSize)
        {
            int totalProduct;
            IEnumerable<Product> products;

            totalProduct = await _productRepository.CountAsync();
            products = await _productRepository.OrderByDescendingBySold(page, pageSize);

            var res = _mapper.Map<IEnumerable<ProductDTO>>(products);
            foreach (var product in res)
            {
                var image = await _imageRepository.GetFirstByProductAsync(product.Id);
                if (image != null)
                {
                    product.ImageUrl = image.ImageUrl;
                }
                var color = await _productColorRepository.GetFirstColorByProductAsync(product.Id);
                if (color != null)
                {
                    product.Price = color.Prices;
                }
                var detail = (await _productDetailRepository.GetDetailProductAsync(product.Id)).FirstOrDefault();
                if (detail != null)
                {
                    product.SizeScreen = detail.SizeScreen;
                    product.Ram = detail.Ram;
                    product.Rom = detail.Rom;
                    product.Cpu = detail.Cpu;
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

        private Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = expr1.Parameters[0];
            var body = Expression.AndAlso(expr1.Body, Expression.Invoke(expr2, parameter));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public async Task<PageRespone<ProductDTO>> GetFilterProductAsync(Filters filters)
        {
            try
            {
                int totalProduct = 0;
                IEnumerable<Product> products = [];
                Expression<Func<Product, bool>> expression = e => e.Enable;

                if (filters.MinPrice != null && filters.MaxPrice != null)
                {
                    expression = CombineExpressions(expression, e =>
                        (e.Product_Colors.FirstOrDefault().Prices - (e.Product_Colors.FirstOrDefault().Prices * (e.Discount / 100.0))) >= filters.MinPrice
                        &&
                        (e.Product_Colors.FirstOrDefault().Prices - (e.Product_Colors.FirstOrDefault().Prices * (e.Discount / 100.0))) <= filters.MaxPrice
                    );
                }
                else
                {
                    if (filters.MinPrice != null)
                    {
                        expression = CombineExpressions(expression, e => (e.Product_Colors.FirstOrDefault().Prices - (e.Product_Colors.FirstOrDefault().Prices * (e.Discount / 100.0))) >= filters.MinPrice);
                    }
                    else if (filters.MaxPrice != null)
                    {
                        expression = CombineExpressions(expression, e => (e.Product_Colors.FirstOrDefault().Prices - (e.Product_Colors.FirstOrDefault().Prices * (e.Discount / 100.0))) <= filters.MaxPrice);
                    }
                }

                if (filters.CategoryIds != null && filters.CategoryIds.Count() > 0)
                {
                    expression = CombineExpressions(expression, e => filters.CategoryIds.Contains(e.CategoryId));
                }

                if (filters.BrandIds != null && filters.BrandIds.Count() > 0)
                {
                    expression = CombineExpressions(expression, e => filters.BrandIds.Contains(e.BrandId));
                }

                if (filters.Ram != null && filters.Ram.Count() > 0)
                {
                    expression = CombineExpressions(expression, e => filters.Ram.Contains(e.Details.Ram));
                }

                if (!string.IsNullOrEmpty(filters.search))
                {
                    var input = filters.search.Trim().Split(' ').Select(x => x.ToLower());

                    expression = CombineExpressions(expression, e => input.All(x => e.Name.ToLower().Contains(x)));
                }

                totalProduct = await _productRepository.CountAsync(expression);
                Expression<Func<Product, double>> priceExp = e => e.Product_Colors.FirstOrDefault().Prices - (e.Product_Colors.FirstOrDefault().Prices * (e.Discount / 100.00));

                switch (filters.Sorter)
                {
                    case SortEnum.PRICE_ASC:
                        products = await _productRepository
                           .GetPagedAsync(filters.page, filters.pageSize, expression, priceExp);
                        break;
                    case SortEnum.PRICE_DESC:
                        products = await _productRepository
                           .GetPagedOrderByDescendingAsync(filters.page, filters.pageSize, expression, priceExp);
                        break;
                    //case SortEnum.NAME:
                    //    products = await _productRepository
                    //       .GetPagedAsync(filters.page, filters.pageSize, expression, e => e.CreateAt);
                    //    break;
                    default:
                        products = await _productRepository
                           .GetPagedOrderByDescendingAsync(filters.page, filters.pageSize, expression, e => e.CreateAt);
                        break;
                }
                var res = _mapper.Map<IEnumerable<ProductDTO>>(products).ToList();

                foreach (var product in res)
                {
                    var image = await _imageRepository.GetFirstByProductAsync(product.Id);
                    if (image != null)
                    {
                        product.ImageUrl = image.ImageUrl;
                    }
                    var color = await _productColorRepository.GetFirstColorByProductAsync(product.Id);
                    if (color != null)
                    {
                        product.Price = color.Prices;
                    }
                    var detail = (await _productDetailRepository.GetDetailProductAsync(product.Id)).FirstOrDefault();
                    if (detail != null)
                    {
                        product.SizeScreen = detail.SizeScreen;
                        product.Ram = detail.Ram;
                        product.Rom = detail.Rom;
                        product.Cpu = detail.Cpu;
                    }
                }

                //var distinctProducts = res
                //    .GroupBy(p => p.Name)
                //    .Select(g => g.OrderBy(p => p.Price).First())
                //    .ToList();

                return new PageRespone<ProductDTO>
                {
                    Items = res,
                    Page = filters.page,
                    PageSize = filters.pageSize,
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
            //var product = await _productRepository.GetProductById(id);
            var product = await _productRepository.SingleOrDefaultAsync(e => e.Id == id);
            if (product != null)
            {
                var res = _mapper.Map<ProductDetailRespone>(product.Details);
                res.ImageUrls = product.Images.Select(x => x.ImageUrl);
                res.Color = _mapper.Map<IEnumerable<ColorResponse>>(product.Product_Colors);
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
                    product.Details.Gpu = request.Details.Gpu;
                    product.Details.Ram = request.Details.Ram;
                    product.Details.Rom = request.Details.Rom;
                    product.Details.Battery = request.Details.Battery;
                    product.Details.size = request.Details.size;
                    product.Details.weight = request.Details.weight;
                    product.Details.version = request.Details.version;
                    product.Details.line = request.Details.line;

                    var existingProductColors = await _productColorRepository.GetColorProductAsync(id);
                    var existingColorIds = existingProductColors.Select(pc => pc.ColorId).ToList();
                    var requestColorIds = request.Color.Select(c => c.ColorId).ToList();                    

                    var colorsToRemove = existingProductColors.Where(pc => !requestColorIds.Contains(pc.ColorId)).ToList();
                    if (colorsToRemove.Any())
                    {
                        await _productColorRepository.DeleteAsync(colorsToRemove);
                    }

                    foreach (var colorRequest in request.Color)
                    {
                        var existingColor = existingProductColors.FirstOrDefault(pc => pc.ColorId == colorRequest.ColorId);
                        if (existingColor != null)
                        {
                            existingColor.Prices = colorRequest.Prices;
                            existingColor.Quantity = colorRequest.Quantity;

                            if (colorRequest.File != null)
                            {
                                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(colorRequest.File.FileName);
                                existingColor.ImageUrl = Path.Combine(path1, newFileName);

                                await _fileStorage.SaveAsync(path1, new List<IFormFile> { colorRequest.File }, new List<string> { newFileName });
                            }
                        }
                        else
                        {
                            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(colorRequest.File.FileName);
                            var newProductColor = new Product_Color
                            {
                                ProductId = product.Id,
                                ColorId = colorRequest.ColorId,
                                Prices = colorRequest.Prices,
                                Quantity = colorRequest.Quantity,
                                ImageUrl = Path.Combine(path1, newFileName)
                            };

                            await _productColorRepository.AddAsync(newProductColor);
                            await _fileStorage.SaveAsync(path1, new List<IFormFile> { colorRequest.File }, new List<string> { newFileName });
                        }
                    }

                    var oldImg = await _imageRepository.GetImageProductAsync(id);
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

        public async Task<bool> UpdateEnableRequest(int id, UpdateEnableRequest request)
        {
            var product = await _productRepository.FindAsync(id);
            if (product != null)
            {
                product.Enable = request.Enable;
                await _productRepository.UpdateAsync(product);
                return product.Enable;
            }
            else throw new ArgumentException($"ID {id} " + ErrorMessage.NOT_FOUND);
        }

        public async Task<IEnumerable<VersionDTO>> GetAllProductVersionsAsync(string? request)
        {
            try
            {
                //if (string.IsNullOrEmpty(request))
                //{
                //    throw new ArgumentException("Search parameter cannot be null or empty.");
                //}
                var products = await _productRepository.SearchAsync(request);
                var res = _mapper.Map<IEnumerable<VersionDTO>>(products);
                foreach (var product in res)
                {
                    var color = await _productColorRepository.GetFirstColorByProductAsync(product.Id);
                    if (color != null)
                    {
                        product.Price = color.Prices;
                    }

                    var detail = (await _productDetailRepository.GetDetailProductAsync(product.Id)).FirstOrDefault();
                    if (detail != null)
                    {
                        product.Version = detail.version;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<IEnumerable<ColorDTO>> GetColorById(int id)
        {
            var color = await _productColorRepository.GetColorProductAsync(id);
            return _mapper.Map<IEnumerable<ColorDTO>>(color);
        }

        public async Task<IEnumerable<NameDTO>> GetNameProduct()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<NameDTO>>(products);
        }

        public async Task<PageRespone<ReviewDTO>> GetReview(int id, PageResquest resquest)
        {
            var review = await _reviewRepository
                .GetPagedOrderByDescendingAsync(resquest.page, resquest.pageSize, e => e.ProductId == id, e => e.CreateAt);

            var total = await _reviewRepository.CountAsync(e => e.ProductId == id);
            var items = _mapper.Map<IEnumerable<ReviewDTO>>(review);

            return new PageRespone<ReviewDTO>
            {
                Items = items,
                Page = resquest.page,
                PageSize = resquest.pageSize,
                TotalItems = total,
            };
        }
    }
}
