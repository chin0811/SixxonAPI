namespace SixxonAPI.Interfaces;

using SixxonAPI.Dtos;
using SixxonAPI.Models;

public interface IToolRepository
{
    Task<IEnumerable<Tool>> GetAllAsync();
    Task<bool> IsBarcodeExistsAsync(string barcode);
    Task InsertAsync(CreateToolDto dto);
}
