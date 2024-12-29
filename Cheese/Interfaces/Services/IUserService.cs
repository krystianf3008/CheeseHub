using CheeseHub.Models.User;
using CheeseHub.Models.User.DTOs;

namespace CheeseHub.Interfaces.Services
{
    public interface IUserService : IBaseService<User>
    {
        public Task<User?> RegisterUser(RegisterUserDTO model);
        public Task<bool> IsUserExistsByEmail(string email);
        public Task<User?> GetByEmail(string email);
        public Task<User?> GetWithRole(string email = null, Guid? id = null);
        public Task<bool?> IsTokenValid(Guid id);
        public Task ToogleTokenValid(Guid id, bool state);
    }
}
