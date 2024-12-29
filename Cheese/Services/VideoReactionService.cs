using CheeseHub.Data;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.User;
using CheeseHub.Models.VideoReaction;

namespace CheeseHub.Services
{
    public class VideoReactionService : ReactionService<VideoReaction>, IVideoReactionService
    {
        public VideoReactionService(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
