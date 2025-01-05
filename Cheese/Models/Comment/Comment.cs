using System.ComponentModel.DataAnnotations;

namespace CheeseHub.Models.Comment
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid VideoId { get; set; }
        public Guid UserId { get; set; }
        public virtual User.User User { get; set; }
        [MaxLength(500)]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ParentCommentId { get; set; } 
        public Video.Video Video { get; set; }
        public virtual Comment ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; }
        public ICollection<CommentReaction.CommentReaction> Reactions { get; set; }

    }
}
