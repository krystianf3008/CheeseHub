using CheeseHub.Interfaces.Models;
using CheeseHub.Models.Role;
using System.ComponentModel.DataAnnotations;

namespace CheeseHub.Models.User
{
    public class User : IModelWithNameAndId
    {
        public Guid Id { get; set; }
        [StringLength(150)]
        [Required]
        public string Name { get; set; }
        [StringLength(150)]
        [Required]
        public string FirstName { get; set; }
        [StringLength(150)]
        [Required]
        public string Surname { get; set; }
        [StringLength(150)]
        [Required]
        public string Email { get; set; }
        [StringLength(150)]
        [Required]
        public string PasswordHash { get; set; }
        public bool? IsTokenValid { get; set; }
        public char Status {  get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid RoleId { get; set; }
        public virtual Role.Role Role { get; set; }
        public ICollection<RefreshToken.RefreshToken> RefreshTokens { get; set; }
        public ICollection<Video.Video> Videos { get; set; }
        public ICollection<VideoView.VideoView> VideoViews { get; set; }
        public ICollection<Comment.Comment> Comments { get; set; }

    }
}
