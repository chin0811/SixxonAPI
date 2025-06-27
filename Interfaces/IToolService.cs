using sixxonAPI.Dtos;
using SixxonAPI.Dtos;

namespace sixxonAPI.Interfaces
{
    public interface IToolService
    {
        Task<IEnumerable<ToolDto>> GetAllAsync();
        Task InsertToolAsync(CreateToolDto dto);
    }

}
