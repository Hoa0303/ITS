using ITS_BE.DTO;

namespace ITS_BE.Services.Colors
{
    public interface IColorService
    {
        Task<IEnumerable<ColorDTO>> GetColors();
        Task<ColorDTO> AddColorsAsync(string name);
        Task<ColorDTO> UpdateColorsAsync(int id, string name);
        Task DeleteColorsAsync(int id);
    }
}
