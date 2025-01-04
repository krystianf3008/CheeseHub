using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Category.DTOs;
using FluentValidation;

namespace CheeseHub.Models.Category.Validators
{
    public class CreateCategoryDTOValidator : AbstractValidator<CreateCategoryDTO>
    {
        public CreateCategoryDTOValidator(ICategoryService categoryService)
        {
            RuleFor(x => x.Name).Must(x => categoryService.IsNameUnique(x)).WithMessage("Name must be unique");
         
        }
    }
}
