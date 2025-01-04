
namespace CheeseHub.Models.Video.DTOs
{
    public class VideoListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string ImagePath { get; set; }
        public int TotalLikes { get; set; }
        public int TotalViews { get; set; }
        public int TotalDisLikes { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public char Status { get; set; }
    }
}
