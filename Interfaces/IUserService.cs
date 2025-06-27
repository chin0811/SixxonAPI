using sixxonAPI.Dtos;

namespace sixxonAPI.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<int> DeleteUserAsync(string id);
        Task<int> CreateUserAsync(UserCreate dto, string ip);
        Task<bool> UpdateUserAsync(string id, UserUpdate dto, string ip);

    }
}
