using CheeseHub.Data;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.User;
using BCrypt;
using CheeseHub.Models.Role;
using CheeseHub.Models.User.DTOs;
using Microsoft.EntityFrameworkCore;
using CheeseHub.Models.RefreshToken;
namespace CheeseHub.Services
{
    public class UserService : BaseService<User>,IUserService
    {
        ApplicationDbContext ApplicationDbContext { get; set; }
        IRoleService RoleService { get; set; }
        public UserService(ApplicationDbContext context, IRoleService roleService) : base(context) 
        {
            ApplicationDbContext = context;
            RoleService = roleService;
        }
        public async Task<User?> RegisterUser(RegisterUserDTO model)
        {
            Guid? roleId = null;
            Role? role = (await RoleService.GetByName("User"));
            if (role == null)
            {
                Role newRole = new Role
                {
                    Name = "User"
                };
                await RoleService.Add(newRole);
                roleId = newRole.Id;
            }
            else
            {
                roleId = role.Id;
            }

            User user = new User
            {
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Name = model.Name,
                Surname = model.Surname,
                FirstName = model.FirstName,
                RoleId = roleId.Value
                

            };
            await ApplicationDbContext.Set<User>().AddAsync(user);
            await ApplicationDbContext.SaveChangesAsync();
            return user;
        }
        public async Task<bool> IsUserExistsByEmail(string email)
        {
            return ApplicationDbContext.Set<User>().Where(u => u.Email.ToLower() == email.ToLower()).Any();
        }
        public async Task<User?> GetByEmail(string email)
        {
            return ApplicationDbContext.Set<User>().Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault();
        }
        public async Task<bool?> IsTokenValid(Guid id)
        {
            return ApplicationDbContext.Set<User>().Where(u => u.Id== id).FirstOrDefault()?.IsTokenValid;
        }
        public async Task ToogleTokenValid(Guid id, bool state)
        {

           User user = ApplicationDbContext.Set<User>().Where(u => u.Id == id).FirstOrDefault();
            if (user != null)
            {
                user.IsTokenValid = state;
            }
            if(!state)
            {
                ApplicationDbContext.Set<RefreshToken>().Where(x => x.UserId == id && x.ExpiryDate > DateTime.UtcNow).ToList().ForEach(y => y.ExpiryDate = DateTime.UtcNow);
                await ApplicationDbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("");
            }

        }
        public async Task<User?> GetWithRole(string email = null, Guid? id = null)
        {
            if (id != null)
            {
                return await ApplicationDbContext.Users.Include(u => u.Role).Where(u => u.Id == id).FirstOrDefaultAsync();

            }
            if (email == null)
            {
                return  await ApplicationDbContext.Users.Include(u => u.Role).AsNoTracking().Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();

            }
            return null;
        }
    }
}
