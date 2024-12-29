using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Role.DTOs;
using CheeseHub.Models.User.DTOs;
using CheeseHub.Services;
using FluentValidation;

namespace CheeseHub.Models.User.Validators
{
    public class RegisterUserDTOValidator : AbstractValidator<RegisterUserDTO>
    {
        public RegisterUserDTOValidator(IUserService userService) 
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150).MustAsync((x, cancellation) => userService.IsNameUnique(x)).WithMessage("Name must be unique");
            RuleFor(x => x.Email).NotEmpty().MaximumLength(150).MustAsync(async (x, cancellation) => !(await userService.IsUserExistsByEmail(x)) ).WithMessage("Email must be unique");
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).Equal(x => x.ConfirmPassword);
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Surname).NotEmpty().MaximumLength(150);
            
        }
    }
}
