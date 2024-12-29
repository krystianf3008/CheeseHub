using CheeseHub.Data;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.CommentReaction;
using CheeseHub.Models.VideoReaction;

namespace CheeseHub.Services
{
    public class CommentReactionService : ReactionService<CommentReaction>, ICommentReactionService
    {
        public CommentReactionService(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
