using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Role.DTOs;
using CheeseHub.Services;
using FluentValidation;

namespace CheeseHub.Models.Role.Validators
{
    public class CreateRoleDTOValidator : AbstractValidator<CreateRoleDTO>
    {
        public CreateRoleDTOValidator(IRoleService roleService)
        {
            RuleFor(x => x.Name).Must(x => roleService.IsNameUnique(x)).WithMessage("Name must be unique");
        }
    }
}
