namespace CheeseHub.Models.Video.DTOs
{
    public class CreateVideoDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public IFormFile File { get; set; }
        public IFormFile Image { get; set; }
    }
}
