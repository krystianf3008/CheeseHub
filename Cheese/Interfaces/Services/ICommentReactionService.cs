using CheeseHub.Models.Comment;
using CheeseHub.Models.CommentReaction;
using CheeseHub.Models.User;

namespace CheeseHub.Interfaces.Services
{
    public interface ICommentReactionService : IReactionService<CommentReaction>
    {
    }
}
