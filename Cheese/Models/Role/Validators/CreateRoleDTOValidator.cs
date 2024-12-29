using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Role.DTOs;
using FluentValidation;

namespace CheeseHub.Models.Role.Validators
{
    public class CreateRoleDTOValidator : AbstractValidator<CreateRoleDTO>
    {
        public CreateRoleDTOValidator(IRoleService roleService)
        {
            RuleFor(x => x.Name).MustAsync((x, cancellation) => roleService.IsNameUnique(x)).WithMessage("Name must be unique");
        }
    }
}
