using ITS_BE.DTO;

namespace ITS_BE.Services.Categories
{
    public interface ICateroryService
    {
        Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
        Task<CategoryDTO> AddCategory(string name);
        Task<CategoryDTO> UpdateCategory(int id, string name);
        Task DeleteCategory(int id);
    }
}
