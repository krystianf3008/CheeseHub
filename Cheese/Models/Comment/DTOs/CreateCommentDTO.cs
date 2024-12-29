namespace CheeseHub.Models.Comment.DTOs
{
    public class CreateCommentDTO
    {
        public Guid?  ParentId { get; set; }
        public string Content { get; set; }
    }
}
