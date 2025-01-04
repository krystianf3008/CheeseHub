using CheeseHub.Interfaces.Models;
using System.ComponentModel.DataAnnotations;

namespace CheeseHub.Models.Video
{
    public class   Video : IModelWithNameAndId
    {
        public Guid Id { get; set; }
        [StringLength(150)]
        [Required]
        public string Name { get; set; }
        [StringLength(4000)]
        public string Description { get; set; }
        [StringLength(150)]

        public string Path { get; set; }
        public string ImagePath { get; set; }
        public Guid UserId { get; set; }
        public virtual User.User User { get; set; }
        public Guid CategoryId { get; set; }
        public virtual Category.Category Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public char Status { get; set; }
        public ICollection<Comment.Comment> Comments { get; set; }
        public ICollection<VideoReaction.VideoReaction> Reactions { get; set; }
        public ICollection<VideoView.VideoView> Views { get; set; }


    }
}
