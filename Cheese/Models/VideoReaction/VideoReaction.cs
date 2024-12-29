using CheeseHub.Interfaces.Models;

namespace CheeseHub.Models.VideoReaction
{
    public class VideoReaction : IReactionModel
    {
        public Guid Id { get; set; }
        public Guid TargetId { get; set; }
        public Guid UserId { get; set; }
        public bool IsLike { get; set; } // true = like, false = dislike

        public virtual Video.Video Video { get; set; }
        public virtual User.User User { get; set; }
    }
}
