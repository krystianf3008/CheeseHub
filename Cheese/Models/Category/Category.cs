using CheeseHub.Interfaces.Models;
using System.ComponentModel.DataAnnotations;

namespace CheeseHub.Models.Category
{
    public class Category : IModelWithNameAndId
    {
        public Guid Id { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(150)]

        public string ImagePath { get; set; }   
        public ICollection<Video.Video> Videos { get; set; }   

    }
}
