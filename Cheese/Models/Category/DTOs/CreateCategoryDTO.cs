namespace CheeseHub.Models.Category.DTOs
{
    public class CreateCategoryDTO
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
    }
}
