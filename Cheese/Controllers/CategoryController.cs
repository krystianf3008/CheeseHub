using CheeseHub.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using CheeseHub.Extensions;
using CheeseHub.Models.Video.DTOs;
using CheeseHub.Models.User;
using CheeseHub.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CheeseHub.Models.SharedDtos;
using CheeseHub.Enums;
using CheeseHub.Models.Category;
using CheeseHub.Models.Category.DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Attributes;
using CheeseHub.Models.User.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using CheeseHub.Models.Category.Validators;

namespace CheeseHub.Controllers
{
    [ApiController]
    [AutoValidation]
    [Route("api/[controller]")]


    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;

        public CategoryController(ICategoryService categoryService, IWebHostEnvironment environment)
        {
            _categoryService = categoryService;
            _environment = environment;
        }
        [HttpGet("{id}", Name = "GetCategory")]
        public async Task<IActionResult> Get([FromRoute] Guid Id)
        {
            GetCategoryDTO? category = await _categoryService.GetCategory(Id);
            if (category == null)
            {
                return NotFound("Category is null");
            }

            return Ok(category);
        }
        [HttpGet(Name = "GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            IQueryable<GetCategoryDTO> categories = await _categoryService.GetCategories();

            return Ok(categories);
        }
        [HttpGet("GetImage/{id}", Name = "GetCategoryImage")]
        public async Task<IActionResult> GetImage(Guid Id)
        {

            Category Category = await _categoryService.GetById(Id);
            if (Category == null)
            {
                return NotFound("Category is null");
            }
            var fileInfo = new FileInfo(Category.ImagePath);
            return File(
                new FileStream(Category.ImagePath, FileMode.Open, FileAccess.Read, FileShare.Read),
                 MimeType.GetMimeType(fileInfo.Extension),
                fileInfo.Name
            );
        }
        [HttpPost]
        [Authorize(Roles ="Admin") ]
        public async Task<IActionResult> Post(CreateCategoryDTO category)
        {
            CreateCategoryDTOValidator validator = new CreateCategoryDTOValidator(_categoryService);
            var result = validator.Validate(category);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            if (category == null || category.Image == null)
            {
                return BadRequest("Invalid category data");
            }

            Guid categoryId = Guid.NewGuid();

            string uploadsFolder = Path.Combine( "CategoriesCovers");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            
            string fileName = $"{categoryId}{Path.GetExtension(category.Image.FileName)}";
            string filePath = Path.Combine(uploadsFolder, fileName);
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    category.Image.CopyTo(stream);
                }
                Category newCategory= new Category
                {
                    Id = categoryId,
                    Name = category.Name,
                    ImagePath = filePath,
                };

                await _categoryService.Add(newCategory);


                return Created(
                    );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



    }
}
