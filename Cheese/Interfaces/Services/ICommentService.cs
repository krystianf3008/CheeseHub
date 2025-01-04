using CheeseHub.Models.Comment.DTOs;
using CheeseHub.Models.Video;
using NuGet.Protocol.Plugins;

namespace CheeseHub.Interfaces.Services
{
    public interface ICommentService
    {
        public Task<GetCommentDTO> Add(CreateCommentDTO createCommentDTO, Guid userId);
        public Task<IQueryable<GetCommentDTO>> GetAllForVideo(Guid VideoId,Guid? userId);
        public Task Delete(Guid userId, Guid id);
    }
}
