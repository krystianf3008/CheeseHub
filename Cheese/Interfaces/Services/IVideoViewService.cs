namespace CheeseHub.Interfaces.Services
{
    public interface IVideoViewService
    {
        public Task AddView(Guid videoId,Guid? userId);
    }
}
