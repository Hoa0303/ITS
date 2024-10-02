using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.BrandRepository;
using ITS_BE.Storage;

namespace ITS_BE.Services.Brands
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly IFileStorage _fileStorage;
        private readonly string path = "assets/images/brands";

        public BrandService(IBrandRepository brandRepository, IMapper mapper, IFileStorage fileStorage)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _fileStorage = fileStorage;
        }

        public async Task<BrandDTO> CreateBrand(string name, IFormFile img)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);

                Brand brand = new()
                {
                    Name = name,
                    ImgUrl = Path.Combine(path, fileName),
                };
                await _brandRepository.AddAsync(brand);
                await _fileStorage.SaveAsync(path, img, fileName);
                return _mapper.Map<BrandDTO>(brand);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task DeleteBrand(int id)
        {
            try
            {
                var brand = await _brandRepository.FindAsync(id);
                if (brand != null)
                {
                    _fileStorage.Delete(brand.ImgUrl);
                    await _brandRepository.DeleteAsync(id);
                }
                else throw new Exception($"ID {id} " + ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<IEnumerable<BrandDTO>> GetAllBrand()
        {
            var brands = await _brandRepository.GetAllAsync();
            var sortedBrands = brands.OrderBy(brand => brand.CreateAt);
            return _mapper.Map<IEnumerable<BrandDTO>>(sortedBrands);
        }

        public async Task<BrandDTO> UpdateBrand(int id, string Name, IFormFile? img)
        {
            var brand = await _brandRepository.FindAsync(id);
            if (brand != null)
            {
                brand.Name = Name;
                if (img != null)
                {
                    _fileStorage.Delete(brand.ImgUrl);
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                    brand.ImgUrl = Path.Combine(path, fileName);

                    await _fileStorage.SaveAsync(path, img, fileName);
                }
                await _brandRepository.UpdateAsync(brand);
                return _mapper.Map<BrandDTO>(brand);
            }
            else { throw new ArgumentException(ErrorMessage.NOT_FOUND); }
        }
    }
}
