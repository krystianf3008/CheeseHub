using CheeseHub.Models.Video;
using CheeseHub.Models.Video.DTOs;

namespace CheeseHub.Interfaces.Services
{
    public interface IVideoService : IBaseService<Video>
    {
        Task<IQueryable<VideoListDTO>> GetVidosWithPagination(string search = null, int pageSize = 10, int page = 0, string categoryId = null, Guid? userId = null, bool? liked = null, bool admin = false);
        Task<SingleVideoDTO?> GetSingleVideo(Guid id, Guid? userId, bool admin = false);
        Task Hide(Guid id, Guid UserId);
        Task Show(Guid id, Guid UserId);
        Task Ban(Guid id);
        Task Unban(Guid id);

    }
}
