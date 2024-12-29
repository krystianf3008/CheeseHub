using CheeseHub.Interfaces.Models;

namespace CheeseHub.Models.CommentReaction
{
    public class CommentReaction : IReactionModel
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public Guid UserId { get; set; }
        public bool IsLike { get; set; } // true = like, false = dislike

        public virtual Comment.Comment Comment { get; set; }
        public virtual User.User User { get; set; }

    }
}
