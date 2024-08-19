using AutoMapper;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.CategoryRepository;

namespace ITS_BE.Services.Categories
{
    public class CategoryService : ICateroryService
    {
        private readonly ICateroryRepository _cateroryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICateroryRepository cateroryRepository, IMapper mapper)
        {
            _cateroryRepository = cateroryRepository;
            _mapper = mapper;
        }
        public async Task<CategoryDTO> AddCategory(string name)
        {
            var categories = new Category
            {
                Name = name
            };
            await _cateroryRepository.AddAsync(categories);
            return _mapper.Map<CategoryDTO>(categories);
        }

        public async Task DeleteCategory(int id) => await _cateroryRepository.DeleteAsync(id);

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            var categories = await _cateroryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> UpdateCategory(int id, string name)
        {
            var category = await _cateroryRepository.FindAsync(id);
            if (category != null)
            {
                category.Name = name;
                await _cateroryRepository.UpdateAsync(category);
                return _mapper.Map<CategoryDTO>(category);
            }
            else throw new ArgumentException($"Id {id} is not valid.");
        }
    }
}
