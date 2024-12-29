using CheeseHub.Models.Video;
using CheeseHub.Models.Video.DTOs;

namespace CheeseHub.Interfaces.Services
{
    public interface IVideoService : IBaseService<Video>
    {
        Task<IQueryable<VideoListDTO>> GetVidosWithPagination(string search = null, int pageSize = 10, int page = 0);
    }
}
