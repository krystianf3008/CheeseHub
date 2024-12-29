using System.ComponentModel.DataAnnotations;

namespace CheeseHub.Models.User.DTOs
{
    public class GetUserDTO
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
