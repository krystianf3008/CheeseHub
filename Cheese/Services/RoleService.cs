using CheeseHub.Data;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Role;

namespace CheeseHub.Services
{
    public class RoleService : BaseService<Role>, IRoleService
    {
        public RoleService(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
