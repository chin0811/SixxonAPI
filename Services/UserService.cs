using Dapper;
using Oracle.ManagedDataAccess.Client;
using sixxonAPI.Dtos;
using sixxonAPI.Interfaces;
using SixxonAPI.Interfaces;
using SixxonAPI.Models;
using System.Data;

namespace sixxonAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var result = await _userRepo.GetAllUserFlatsAsync();

            var grouped = result
                .GroupBy(r => new { r.UserId, r.Account, r.Username, r.Email, r.Visible })
                .Select(g => new UserDto
                {
                    UserId = g.Key.UserId,
                    Account = g.Key.Account,
                    Username = g.Key.Username,
                    Email = g.Key.Email,
                    Visible = g.Key.Visible,
                    Roles = g
                        .Where(x => !string.IsNullOrEmpty(x.RoleId))
                        .Select(x => new RoleDto
                        {
                            RoleId = x.RoleId,
                            RoleName = x.RoleName
                        })
                        .Distinct()
                        .ToList()
                });

            return grouped;
        }
        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _userRepo.GetAllRolesAsync();

            return roles.Select(r => new RoleDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName
            });
        }
        public async Task<int> DeleteUserAsync(string id)
        {
            return await _userRepo.DeleteUserAsync(id);
        }
        public async Task<int> CreateUserAsync(UserCreate dto , string ip)
        {
            return await _userRepo.CreateUserAsync(dto , ip);
        }
        public async Task<bool> UpdateUserAsync(string id, UserUpdate dto, string ip)
        {
            return await _userRepo.UpdateUserAsync(id, dto, ip);
        }

    }
}
