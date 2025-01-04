using CheeseHub.Interfaces.Models;

namespace CheeseHub.Models.Category
{
    public class Category : IModelWithNameAndId
    {
        public Guid Id { get; set; }
        public string Name { get; set; }   
        public string ImagePath { get; set; }   
        public ICollection<Video.Video> Videos { get; set; }   

    }
}
