using CheeseHub.Enums;
using CheeseHub.Interfaces.Models;

namespace CheeseHub.Interfaces.Services
{
    public interface IReactionService<T> where T : IReactionModel
    {
        public Task ToggleReaction(Guid UserId, Guid TargetId, ToggleReactionEnum status);
    }
}
