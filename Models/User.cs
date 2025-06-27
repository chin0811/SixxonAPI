namespace SixxonAPI.Models
{
    public class User
    {
        public string UserId { get; set; }                     // USER_ID
        public string Username { get; set; }                   // USERNAME
        public string Email { get; set; }                      // EMAIL
        public string PasswordHash { get; set; }               // PASSWORDHASH
        public int EmailConfirmed { get; set; }                // EMAILCONFIRMED 
        public bool Visible { get; set; }                      // VISIBLE (oracle沒有bool)
        public int? Country { get; set; }                      // COUNTRY (nullable)
        public int? FactoryId { get; set; }                    // FACTORYID (nullable)
        public DateTime InsertTime { get; set; }               // INSERTTIME
        public string InsertIp { get; set; }                   // INSERTIP
        public string InsertUser { get; set; }                 // INSERTUSER
        public DateTime? UpdateTime { get; set; }              // UPDATETIME 
        public string UpdateUser { get; set; }                 // UPDATEUSER
        public string UpdateIp { get; set; }                   // UPDATEIP
        public int? ConstructionEnterpriseId { get; set; }     // CONSTRUCTIONENTERPRISEID 
        public string Account { get; set; }                    // ACCOUNT

        public List<Role> Roles { get; set; } = new(); // 加上這個表示包含關聯資訊
    }

    public class Role
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class UserRole
    {
        public string Id { get; set; } 
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}
