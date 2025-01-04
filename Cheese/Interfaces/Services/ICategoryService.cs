using CheeseHub.Models.Category;
using CheeseHub.Models.Category.DTOs;

namespace CheeseHub.Interfaces.Services
{
    public interface ICategoryService : IBaseService<Category>
    {
        Task<IQueryable<GetCategoryDTO>> GetCategories();
        Task<GetCategoryDTO?> GetCategory(Guid id);
    }
}
