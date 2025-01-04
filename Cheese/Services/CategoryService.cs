using Azure.Core;
using CheeseHub.Data;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Category;
using CheeseHub.Models.Category.DTOs;
using CheeseHub.Models.User;
using CheeseHub.Models.Video;
using CheeseHub.Models.Video.DTOs;
using CheeseHub.Models.VideoReaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Policy;

namespace CheeseHub.Services
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        ApplicationDbContext ApplicationDbContext { get; set; }


        public CategoryService(ApplicationDbContext context) : base(context)
        {
            ApplicationDbContext = context;
        }
        public async Task<IQueryable<GetCategoryDTO>> GetCategories()
        {
            IQueryable<Category> query = ApplicationDbContext.Category.Include(x => x.Videos);

            return query.Select(x => new GetCategoryDTO
            {
                CategoryName = x.Name,
                Id = x.Id,
                VideoCount = x.Videos.Count
            });
        }
        public async Task<GetCategoryDTO?> GetCategory(Guid id)
        {
            Category? category = await ApplicationDbContext.Category.Include(x => x.Videos).Where(x => x.Id == id).FirstOrDefaultAsync();
            if(category == null)
            {
                return null;
            }

            return new GetCategoryDTO
            {
                CategoryName = category.Name,
                Id = category.Id,
                VideoCount = category.Videos.Count
            };
        }
    }
}
