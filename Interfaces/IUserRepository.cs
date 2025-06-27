namespace SixxonAPI.Interfaces;

using sixxonAPI.Dtos;
using SixxonAPI.Models;

public interface IUserRepository
{
    Task<IEnumerable<UserFlat>> GetAllUserFlatsAsync();
    Task<IEnumerable<Role>> GetAllRolesAsync();
    Task<int> DeleteUserAsync(string id);
    Task<int> CreateUserAsync(UserCreate dto, string ip);
    Task<bool> UpdateUserAsync(string id, UserUpdate dto, string ip);

}
