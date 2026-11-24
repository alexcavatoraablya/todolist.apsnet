using Microsoft.AspNetCore.Identity;

namespace WebToDoList.Data.Identity;

public class RoleEntity : IdentityRole<int>
{
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
}
