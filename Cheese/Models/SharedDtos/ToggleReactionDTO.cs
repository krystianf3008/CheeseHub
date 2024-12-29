namespace CheeseHub.Models.SharedDtos
{
    public class ToggleReactionDTO
    {
        public Guid UserId { get; set; }
        public Guid VideoId { get; set; }
        public char ToggleType { get; set; }
    }
}
