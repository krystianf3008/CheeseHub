using CheeseHub.Data;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.VideoView;

namespace CheeseHub.Services
{
    public class VideoViewService : IVideoViewService
    {
        private readonly ApplicationDbContext _dBContext;


        public VideoViewService(ApplicationDbContext dBContext)
        {
            _dBContext = dBContext;
        }
        public async Task AddView(Guid videoId, Guid? userId)
        {
            VideoView videoView = new VideoView
            {
                VideoId = videoId,
                UserId = userId,
                ViewedAt = DateTime.UtcNow,
            };
            await _dBContext.Set<VideoView>().AddAsync(videoView);
            await _dBContext.SaveChangesAsync();

        }
    }
}
