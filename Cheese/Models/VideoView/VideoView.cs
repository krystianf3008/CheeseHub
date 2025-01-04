namespace CheeseHub.Models.VideoView
{
    public class VideoView
    {
        public Guid Id { get; set; }
        public Guid VideoId { get; set; }
        public Guid? UserId { get; set; } 
        public DateTime ViewedAt { get; set; }

        public virtual Video.Video Video { get; set; }
        public virtual User.User? User { get; set; }
    }
}
