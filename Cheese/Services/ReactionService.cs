using CheeseHub.Data;
using CheeseHub.Interfaces.Models;
using CheeseHub.Interfaces.Services;
using CheeseHub.Data;
using CheeseHub.Interfaces.Models;
using CheeseHub.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using CheeseHub.Enums;

namespace CheeseHub.Services
{
    public class ReactionService<T> : IReactionService<T> where T : class, IReactionModel, new()
    {
        private readonly ApplicationDbContext _dBContext;


        public ReactionService(ApplicationDbContext dBContext)
        {
            _dBContext = dBContext;
        }
        public async Task ToggleReaction(Guid UserId, Guid TargetId, ToggleReactionEnum status)
        {
            if(status == ToggleReactionEnum.Undo)
            {
                T? modelToDel = await _dBContext.Set<T>().FirstOrDefaultAsync(x => x.UserId == UserId && x.TargetId == TargetId);
                if (modelToDel != null)
                {
                    _dBContext.Set<T>().Remove(modelToDel);
                    await _dBContext.SaveChangesAsync();
                    
                }
                return;
            }
            bool isLike = status == ToggleReactionEnum.Like;
            T? model = await _dBContext.Set<T>().FirstOrDefaultAsync(x => x.UserId == UserId && x.TargetId == TargetId);
            if (model == null)
            {
                model = new T
                {
                    UserId = UserId,
                    TargetId = TargetId,
                    IsLike = isLike
                };
                await _dBContext.Set<T>().AddAsync(model);


            }
            else
            {
                model.IsLike = isLike;
            }


            await _dBContext.SaveChangesAsync();
        }
    }
}
