namespace CheeseHub.Models.SharedDtos
{
    public class ToggleReactionDTO
    {
        public Guid TargetId { get; set; }
        public char ToggleType { get; set; }
    }
}
