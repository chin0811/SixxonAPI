// Repositories/UserRepository.cs
using Dapper;
using Oracle.ManagedDataAccess.Client;
using sixxonAPI.Dtos;
using sixxonAPI.Interfaces;
using sixxonAPI.Services;
using SixxonAPI.Interfaces;
using SixxonAPI.Models;
using System.Data;

namespace sixxonAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
        }

        private IDbConnection Connection => new OracleConnection(_connectionString);

        //會員資料表
        public async Task<IEnumerable<UserFlat>> GetAllUserFlatsAsync()
        {
            using var conn = Connection;

            var sql = @"
            SELECT 
                u.USER_ID AS UserId,
                u.ACCOUNT AS Account,
                u.USERNAME AS Username,
                u.EMAIL AS Email,
                u.VISIBLE AS Visible,
                r.ROLE_ID AS RoleId,
                r.ROLE_NAME AS RoleName
            FROM ASPNETUSER u
            LEFT JOIN USERROLETABLE ur ON u.USER_ID = ur.USER_ID
            LEFT JOIN ASPNETROLE r ON ur.ROLE_ID = r.ROLE_ID
            ORDER BY u.USER_ID
            ";

            var result = await conn.QueryAsync<UserFlat>(sql);
            return result;
        }
        //角色對應表
        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            using var conn = Connection;

            var sql = "SELECT ROLE_ID AS RoleId, ROLE_NAME AS RoleName FROM ASPNETROLE ORDER BY ROLE_NAME";

            return await conn.QueryAsync<Role>(sql);
        }
        //刪除使用者
        public async Task<int> DeleteUserAsync(string id)
        {
            using var conn = Connection;
            conn.Open(); //改為同步開啟連線

            using var tx = conn.BeginTransaction();

            try
            {
                // 1. 先刪除角色關聯
                var deleteRolesSql = "DELETE FROM USERROLETABLE WHERE USER_ID = :id";
                await conn.ExecuteAsync(deleteRolesSql, new { id }, tx);

                // 2. 再刪使用者本身
                var deleteUserSql = "DELETE FROM ASPNETUSER WHERE USER_ID = :id";
                var result = await conn.ExecuteAsync(deleteUserSql, new { id }, tx);

                tx.Commit();
                return result;
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<int> CreateUserAsync(UserCreate dto, string ip)
        {
            using var conn = Connection;
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                var userId = Guid.NewGuid().ToString();

                // 1. 新增使用者
                var sql = @"
                INSERT INTO ASPNETUSER (
                USER_ID, USERNAME, ACCOUNT, EMAIL, PASSWORDHASH, EMAILCONFIRMED, VISIBLE,
                INSERTTIME, INSERTIP, INSERTUSER
                )
                 VALUES (
                :UserId, :Username, :Account, :Email, :PasswordHash, 1, 1,
                :InsertTime, :InsertIp, :InsertUser
                )";

                var parameters = new
                {
                    UserId = userId,
                    Username = dto.Username,
                    Account = dto.Account,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    InsertTime = DateTime.Now,
                    InsertIp = ip,
                    InsertUser = "system"
                };

                await conn.ExecuteAsync(sql, parameters, transaction);

                // 2. 新增角色關聯（多筆）
                if (dto.RoleIds?.Any() == true)
                {
                    var insertRoleSql = @"
                INSERT INTO USERROLETABLE (ID, USER_ID, ROLE_ID)
                VALUES (:Id, :UserId, :RoleId)";

                    foreach (var roleId in dto.RoleIds)
                    {
                        await conn.ExecuteAsync(insertRoleSql, new
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = userId,
                            RoleId = roleId
                        }, transaction);
                    }
                }

                transaction.Commit();
                return 1;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(string id, UserUpdate dto, string ip)
        {
            using var conn = Connection;
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // 1. 先查舊資料（角色）
                var originalRoles = (await conn.QueryAsync<string>(
                    "SELECT ROLE_ID FROM USERROLETABLE WHERE USER_ID = :UserId",
                    new { UserId = id }, tx)).ToHashSet();

                var newRoles = dto.RoleIds?.ToHashSet() ?? new HashSet<string>();

                var rolesToAdd = newRoles.Except(originalRoles);
                var rolesToRemove = originalRoles.Except(newRoles);

                // 2. 更新使用者基本資料（密碼 optional）
                var sql = string.IsNullOrEmpty(dto.Password)
                    ? @"
                UPDATE ASPNETUSER SET
                    USERNAME = :Username,
                    ACCOUNT = :Account,
                    EMAIL = :Email,
                    UPDATETIME = :UpdateTime,
                    UPDATEUSER = :UpdateUser,
                    UPDATEIP = :UpdateIp
                WHERE USER_ID = :UserId"
                    : @"
                UPDATE ASPNETUSER SET
                    USERNAME = :Username,
                    ACCOUNT = :Account,
                    EMAIL = :Email,
                    PASSWORDHASH = :PasswordHash,
                    UPDATETIME = :UpdateTime,
                    UPDATEUSER = :UpdateUser,
                    UPDATEIP = :UpdateIp
                WHERE USER_ID = :UserId";

                var parameters = new
                {
                    Username = dto.Username,
                    Account = dto.Account,
                    Email = dto.Email,
                    PasswordHash = string.IsNullOrEmpty(dto.Password) ? null : BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    UpdateTime = DateTime.Now,
                    UpdateUser = "system",
                    UpdateIp = ip,
                    UserId = id
                };

                await conn.ExecuteAsync(sql, parameters, tx);

                // 3. 刪除多餘的角色
                foreach (var roleId in rolesToRemove)
                {
                    await conn.ExecuteAsync(
                        "DELETE FROM USERROLETABLE WHERE USER_ID = :UserId AND ROLE_ID = :RoleId",
                        new { UserId = id, RoleId = roleId }, tx);
                }

                // 4. 新增缺少的角色
                foreach (var roleId in rolesToAdd)
                {
                    await conn.ExecuteAsync(
                        "INSERT INTO USERROLETABLE (ID, USER_ID, ROLE_ID) VALUES (:Id, :UserId, :RoleId)",
                        new
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = id,
                            RoleId = roleId
                        }, tx);
                }

                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

    }
}
