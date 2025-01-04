using CheeseHub.Data;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Comment;
using CheeseHub.Models.Comment.DTOs;
using CheeseHub.Models.Video.DTOs;
using CheeseHub.Models.Video;
using CheeseHub.Models.VideoReaction;
using CheeseHub.Models.CommentReaction;
using Microsoft.EntityFrameworkCore;

using CheeseHub.Models.User;

namespace CheeseHub.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _dBContext;


        public CommentService(ApplicationDbContext dBContext)
        {
            _dBContext = dBContext;
        }
        public async Task<GetCommentDTO> Add(CreateCommentDTO createCommentDTO, Guid userId)
        {
            Comment model = new Comment
            {
                Content = createCommentDTO.Content,
                CreatedAt = DateTime.UtcNow,
                ParentCommentId = string.IsNullOrEmpty( createCommentDTO.ParentId ) ? null : Guid.Parse(createCommentDTO.ParentId),
                UserId = userId,
                VideoId = createCommentDTO.VideoId
            };
            await _dBContext.Set<Comment>().AddAsync(model);
            await _dBContext.SaveChangesAsync();
            Comment CommentFromDb = _dBContext.Set<Comment>().Include(x => x.User).FirstOrDefault(x => x.Id == model.Id);
            return new GetCommentDTO
            {
                Id = CommentFromDb.Id,
                ParentId = CommentFromDb.ParentCommentId,
                UserName = CommentFromDb.User.Name,
                TotalLikes = 0,
                TotalDisLikes = 0,
                Content = CommentFromDb.Content,
                CreatedAt = CommentFromDb.CreatedAt,
                IsLiked = null,
                IsDisLiked = null
            };
        }
        public async Task Delete(Guid UserId, Guid id)
        {
            Comment? model = _dBContext.Set<Comment>().Include(x => x.Reactions).FirstOrDefault(x => x.Id == id);
            if (model != null)
            {
         
                if (model.UserId != UserId)
                {
                    throw new UnauthorizedAccessException();
                }
                ICollection<CommentReaction> reactions = model.Reactions;

                _dBContext.CommentReactions.RemoveRange(reactions);
                _dBContext.RemoveRange(_dBContext.Comments.Where(x => x.ParentCommentId == id));

                _dBContext.Set<Comment>().Remove(model);
                await _dBContext.SaveChangesAsync();
            }


        }
        //admin - skip permission check
        public async Task Delete(Guid id)
        {
            _dBContext.Set<Comment>().Remove(_dBContext.Set<Comment>().FirstOrDefault(x => x.Id == id));
            await _dBContext.SaveChangesAsync();

        }
        public async Task<IQueryable<GetCommentDTO>> GetAllForVideo(Guid VideoId, Guid? UserId)
        {
            IQueryable<Comment> comments =  _dBContext.Comments.Include(c => c.User).Include(c => c.Reactions).Where(x => x.VideoId == VideoId);
            IQueryable<GetCommentDTO> model = comments.Select(x => new GetCommentDTO
            {
                Id = x.Id,
                UserId = x.UserId,
                ParentId = x.ParentCommentId,
                UserName = x.User.Name,
                TotalLikes = x.Reactions.Where(r => r.IsLike).Count(),
                TotalDisLikes = x.Reactions.Where(r => !r.IsLike).Count(),
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                IsLiked = UserId == null ? null : x.Reactions.Where(r => r.IsLike && r.UserId == UserId).Count() > 0,
                IsDisLiked = UserId == null ? null : x.Reactions.Where(r => !r.IsLike && r.UserId == UserId).Count() > 0
            });

            return model;


        }

    }
}
