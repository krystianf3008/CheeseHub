using CheeseHub.Interfaces.Models;
using CheeseHub.Models.User;
using System.ComponentModel.DataAnnotations;

namespace CheeseHub.Models.Role
{
    public class Role : IModelWithNameAndId
    {
        public Guid Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public virtual ICollection<User.User> Users { get; set; }  = new List<User.User>();
    }
}
