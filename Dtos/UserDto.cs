namespace sixxonAPI.Dtos
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Account { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Visible { get; set; }
        public List<RoleDto> Roles { get; set; } = new();
    }
    public class RoleDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
    public class UserCreate
    {
        public string Account { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> RoleIds { get; set; }
    }
    public class UserUpdate
    {
        public string Username { get; set; }
        public string Account { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // 可選，如果為 null 不更新
        public List<string> RoleIds { get; set; }
    }
}