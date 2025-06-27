using Microsoft.AspNetCore.Mvc;
using sixxonAPI.Dtos;
using sixxonAPI.Interfaces;

namespace sixxonAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _userService.GetAllRolesAsync();
            return Ok(roles);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (result == 0)
                return NotFound();

            return Ok(new { message = "刪除成功" });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreate dto)
        {
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            var result = await _userService.CreateUserAsync(dto, ip);
            return result > 0 ? Ok("新增成功") : StatusCode(500, "新增失敗");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdate dto)
        {
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                  ?? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();

            var success = await _userService.UpdateUserAsync(id, dto, ip);
            return success ? Ok("編輯成功") : NotFound();
        }

    }
}
