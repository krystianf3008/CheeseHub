namespace CheeseHub.Models.Category.DTOs
{
    public class GetCategoryDTO
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public int VideoCount { get; set; }
    }
}
