using AutoMapper;
using ITS_BE.Constants;
using ITS_BE.DTO;
using ITS_BE.Models;
using ITS_BE.Repository.ColorRespository;

namespace ITS_BE.Services.Colors
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IMapper _mapper;

        public ColorService(IColorRepository colorRepository, IMapper mapper)
        {
            _colorRepository = colorRepository;
            _mapper = mapper;
        }
        public async Task<ColorDTO> AddColorsAsync(string name)
        {
            var color = new Color
            {
                Name = name,
            };
            await _colorRepository.AddAsync(color);
            return _mapper.Map<ColorDTO>(color);
        }

        public async Task DeleteColorsAsync(int id) => await _colorRepository.DeleteAsync(id);

        public async Task<ColorDTO> GetColorById(int id)
        {
            var color = await _colorRepository.FindAsync(id);
            return _mapper.Map<ColorDTO>(color);
        }

        public async Task<IEnumerable<ColorDTO>> GetColors()
        {
            var colors = await _colorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ColorDTO>>(colors);
        }

        public async Task<ColorDTO> UpdateColorsAsync(int id, string name)
        {
            var color = await _colorRepository.FindAsync(id);
            if(color == null)
            {
                throw new ArgumentException($"ID {id}" + ErrorMessage.NOT_FOUND);
            }
            else
            {
                color.Name = name;
                await _colorRepository.UpdateAsync(color);
                return _mapper.Map<ColorDTO>(color);
            }
        }
    }
}
